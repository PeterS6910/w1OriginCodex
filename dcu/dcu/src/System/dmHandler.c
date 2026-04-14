#include "dmHandler.h"
#include "baseTypes.h"





#ifdef DM_HANDLER_MALLOC

    #include <stdlib.h>

    volatile uint32 _memAllocSize = 0;

#else

    #include "LPC122x.h"

    #define MAX_CELL_SIZE 255
    #define MIN_CELL_SIZE 4


    // SIZE OF THE DYNAMIC MEMORY
    //#define PREALLOC_SIZE 992
    //#define PREALLOC_SIZE 984
    #define PREALLOC_SIZE 976
    //#pragma location=0x10001C00   // 8kB - PREALLOC_SIZE - 32 
    #pragma location="DM"
#ifndef XPRESSO
    __no_init volatile unsigned char _memoryBuffer[PREALLOC_SIZE];
#else
	#include <cr_section_macros.h>
    __NOINIT(RAM) volatile unsigned char _memoryBuffer[PREALLOC_SIZE];
#endif
    
    struct CellInfo {
        uint8 size;
        uint8 size2;
        uint8 size3;
        uint8 isAllocated; // allocated flag in different order, so it'll be the MSB
    };
    
    
    typedef struct CellInfo CellInfo_t;
    
    const uint32 _dmCellMarkSize = sizeof(CellInfo_t);
    
    volatile uint32 _dmLastAllocated = 0xFFFF;
    
    #ifndef BOOTLOADER
    volatile uint32 _allocatedSize = 0;
    #endif
    
    #ifdef DEBUG
    volatile uint32 _outOfMemoryCount = 0;
    #endif
    
    /*volatile uint32 _successfulCount = 0;
    volatile uint32 _averageSize = 0;*/
    
    //#define DMH_DEBUG_VERBOSE
    
    #ifdef DMH_DEBUG_VERBOSE
    volatile uint32 _newCount = 0;
    //volatile uint32 _deleteCount = 0;
    //volatile uint32 _cumulationCount = 0;
    volatile uint32 _allocatedCells = 0;
    volatile uint32 _failureCount = 0;
    #endif
    
    //volatile uint32 _dmHandlerFlags = 0x00000000;
    //#define DMH_INITIALIZED     0x80000000
    
    
#endif
    
#ifndef BOOTLOADER
    uint32 MemoryLoad() 
    {
    	#ifndef DM_HANDLER_MALLOC
        	return _allocatedSize*100 / PREALLOC_SIZE;
    	#else
        	return 2; // no way to determine YET
    	#endif
    }
#endif

#ifndef DM_HANDLER_MALLOC

    #ifndef BOOTLOADER
    void  AddAllocationStatistic(uint32 size) {
        //_successfulCount++;
        
    #ifdef DMH_DEBUG_VERBOSE
        _allocatedCells++;
    #endif
        
        //_averageSize += size;
    
        _allocatedSize += size + _dmCellMarkSize;
    }
    
    void RemoveAllocationStatistic(uint32 size) {
    #ifdef DMH_DEBUG_VERBOSE    
        _allocatedCells--;
    #endif
        
    
        if (_allocatedSize >= size + _dmCellMarkSize)
            _allocatedSize -= size + _dmCellMarkSize;
        else {
            _allocatedSize = 0;
        }
    }
    #endif
    
    void SetCellSize(CellInfo_t* cellInfo,uint32 size) {
        if (null != cellInfo)
        {
            cellInfo->size = size;
    #ifdef DEBUG
            cellInfo->size2 = size;
            cellInfo->size3 = size;
    #endif
        }
    }
    
    //void SpaceOptimization(uint32 actualIndex);
    
    
    /**
     * @param start - position in the buffer, where to start
     * @param size - size of the cell to search for
     * @param originalSize - parameter returned in phase 2, if the requested size is smaller than the size of the cell requested
     */
    uint32 FindPosition(uint32 start,uint32* requestedSize) {
        if (_dmLastAllocated == 0xFFFF)
            return 0;
        
        CellInfo_t* pCell;
        
        uint32 actual = start;
       
        // 1.st phase - to avoid fragmentation, look only for perfect match
        while(actual < _dmLastAllocated && (actual + _dmCellMarkSize + *requestedSize - 1 < _dmLastAllocated)) {
            pCell = ((CellInfo_t*)(_memoryBuffer+actual));
            
            if (!pCell->isAllocated) {
        
                //SpaceOptimization(actual);
                
                if (pCell->size == *requestedSize) 
                    return actual;
            }
           
            actual +=  _dmCellMarkSize + pCell->size; // shift to next cell
        }
        
        
        actual = start;
         // 2nd phase - find bigger cells, or try to aggreagte
        while(actual < _dmLastAllocated && (actual + _dmCellMarkSize + *requestedSize - 1 < _dmLastAllocated)) {
            pCell = (CellInfo_t*)(_memoryBuffer+actual);
            
            if (!pCell->isAllocated) {
                if (*requestedSize < pCell->size)  { // no need for equal, checked in first phase
        
                    if (*requestedSize + _dmCellMarkSize + MIN_CELL_SIZE <= pCell->size) {
                        // if there's space for at least 4B empty cell , let it fragment 
                        uint32 fragmentedSize = pCell->size - *requestedSize - _dmCellMarkSize;
                        
                        CellInfo_t* newCell = (CellInfo_t*)(_memoryBuffer + actual + _dmCellMarkSize + *requestedSize);
                        SetCellSize(newCell,fragmentedSize);
                        newCell->isAllocated = 0;
                    }
                    else
                        // use all size of found cell
                        *requestedSize = pCell->size;
                    
                    return actual;
                }
                else {
                    
                    CellInfo_t* pNextCell = (CellInfo_t*)(_memoryBuffer+actual+_dmCellMarkSize+pCell->size);
                    if (!pNextCell->isAllocated) {
                        uint32 newSize = pCell->size + _dmCellMarkSize + pNextCell->size;
                        if (*requestedSize <= newSize) {
                            if (*requestedSize + _dmCellMarkSize  + 4 <= newSize) {
                                // if there's space for at least 4B empty cell , let it fragment 
                                uint32 fragmentedSize = newSize - *requestedSize - _dmCellMarkSize;
                                
                                CellInfo_t* newCell = (CellInfo_t*)(_memoryBuffer + actual +   _dmCellMarkSize + (*requestedSize) );
                                
                                SetCellSize(newCell,fragmentedSize);
                                newCell->isAllocated = 0;
                            }
                            else {                                                      
                                //size of the cell will be overwritten by Internal Alloc
                                *requestedSize = newSize;
                            }
                            
                             if (actual+_dmCellMarkSize+pCell->size == _dmLastAllocated){
                                //shifting the _dmLastAllocated because of defragmentation
                                
                                _dmLastAllocated = actual; // means the relative index of pCell
                                // pCell becomes the last allocated cell
                            }
                            
                            return actual;
                        }
                        else {
                            // defragment 
                            if (actual+_dmCellMarkSize+pCell->size == _dmLastAllocated){
                                //shifting the _dmLastAllocated because of defragmentation
                                
                                _dmLastAllocated = actual; // means the relative index of pCell
                                // pCell becomes the last allocated cell
                            }
                            
                            // null the pNextCell 4B
                            SetCellSize(pNextCell,0);
                            pNextCell->isAllocated = 0;
                            
                            SetCellSize(pCell,newSize);
                            continue;
                        }    
                    }
                    
                }
            }
           
            // go to next
            actual += _dmCellMarkSize + pCell->size;
            
        }
        
        if (_dmLastAllocated == 0xFFFF)
            return 0xFFFF;
        
        // 3rd phase - allocate the newest on top of _dmLastAllocated if possible
        pCell = (CellInfo_t*)(_memoryBuffer+_dmLastAllocated);
        if (!pCell->isAllocated){
            return _dmLastAllocated;
        }
        
        uint32 newPos = _dmLastAllocated + _dmCellMarkSize + pCell->size;
        
        if (newPos + _dmCellMarkSize + *requestedSize - 1 < PREALLOC_SIZE)
            return newPos;
        else    
            /// means nothing found
            return 0xFFFF;
        
        
    }
    
    
    
    void* InternalAlloc(uint32 position, uint32 size) {
        if (position+_dmCellMarkSize + size >PREALLOC_SIZE ||
            size > MAX_CELL_SIZE)
            // should not happen
            return null;
        
    /*  if (size > MAX_CELL_SIZE)
            return null;*/
        
        CellInfo_t* pCell;
        
        pCell = (CellInfo_t*)(_memoryBuffer+position);
        if (position < _dmLastAllocated &&
            pCell->isAllocated) {
                
            // should not happen
            uint32 txx = pCell->size*2;
            txx*=pCell->size;
            
            return null;
        }
        //else
            // if the position is higher than _dmLastAllocated, it could be anything
            // therefore don't verify, if it's not allocated
        
        pCell->isAllocated = 1;
        SetCellSize(pCell,size);
        
        // zero the memory
        for(int32 i=position+_dmCellMarkSize ; i< position + _dmCellMarkSize + size;i++) {      
            _memoryBuffer[i] = 0;
        }
        
        return (void*)(_memoryBuffer+position+_dmCellMarkSize);
    }
    
    /*static inline void EnsureInitialization() {
        if (!(_dmHandlerFlags & DMH_INITIALIZED))
        {
            for(int32 i=0;i<PREALLOC_SIZE;i++)
              _memoryBuffer[i] = 0x00;
            
            _dmHandlerFlags |= DMH_INITIALIZED;
        }
    }*/

#endif

/**
 *
 */

void* New(uint32 size) {
#ifdef DM_HANDLER_MALLOC
    void* ret = calloc(size,1);
    /*if (null != ret)
      _memAllocSize += size;*/
    
    return ret;                    
#else
    //EnsureInitialization();  
  
#ifdef DMH_DEBUG_VERBOSE    
    _newCount++;
#endif
    
    // ENSURE ALIGNMENT TO 4B!!!
    // MARK AND ALSO CELL HAS TO BE ALIGNED TO 4B ADDRESS BECAUSE OF THE POINTERS, THAT CAN BE ALLOCATED WITHIN CELL
    uint32 complement = size % 4;
    if (complement > 0)
      size += (4 - complement);
        
    if (size < MIN_CELL_SIZE) {
#ifdef DMH_DEBUG_VERBOSE
        _failureCount++;
#endif
        return null;
    }
    
    if (size > MAX_CELL_SIZE) {
#ifdef DMH_DEBUG_VERBOSE
        _failureCount++;
#endif
        return null;
    }
    
    void* p;
    
    if (_dmLastAllocated == 0xFFFF) 
    {
        // IMPORTANT !!!!!!!
        // FIRST INITIALIZATION OF THE BUFFER
        for(int32 i=0;i<PREALLOC_SIZE;i++) 
            _memoryBuffer[i] = 0x00;
        // IMPORTANT !!!!!!!
        
        // to ensure first mark in the buffer
        
        p = InternalAlloc(0,size);
        if (p == null) {
#ifdef DMH_DEBUG_VERBOSE
            _failureCount++;
#endif
            return null;
        }
        
        _dmLastAllocated = 0;
        
#ifndef BOOTLOADER
        AddAllocationStatistic(size);
#endif

    }
    else {
           
        uint32 newPos =  FindPosition(0, &size);
        if (newPos == 0xFFFF) {
#ifdef DEBUG
            _outOfMemoryCount++;
#endif
            return null;
        }
        
        p = InternalAlloc(newPos,size);
        if (null == p) {
#ifdef DEBUG
            _outOfMemoryCount++;
#endif
            return null;
        }
                           
        if (newPos > _dmLastAllocated)
            _dmLastAllocated = newPos;
        
        /*if (newPos == _firstDeallocated) {
            FindFirstDeallocated(newPos);
        }*/
        
#ifndef BOOTLOADER
        AddAllocationStatistic(size);
#endif
    }
    
    return p;
#endif
}

/*
void SpaceOptimization(uint32 actualIndex) {
    if (actualIndex + _dmCellMarkSize >= PREALLOC_SIZE)
        return;
    
    // space for optimization
    // cumulate deallocated spaces
    CellInfo_t* pNextCell;
    CellInfo_t* pCell = (CellInfo_t*)(_memoryBuffer + actualIndex);
    
    while(1) {
        
        uint32 newPos = actualIndex + _dmCellMarkSize + pCell->size;
               
        if (newPos >= _dmLastAllocated || newPos >= PREALLOC_SIZE)
            break;
        
        pNextCell = (CellInfo_t*)(_memoryBuffer + newPos);
        if (pNextCell->size == 0)
            // posilbly end marker
            break;
        
        if (!pNextCell->isAllocated) {
            if (pCell->size +_dmCellMarkSize + pNextCell->size  <= MAX_CELL_SIZE) { 
            
                pCell->size += _dmCellMarkSize + pNextCell->size; // includes starting mark
            
                // similar to end marker
                pNextCell->size = 0;
                //pNextCell->reserved = 0;
                
            }
            else 
                // nothing to cumulate
                // avoid overflow on the 7bit size
                break;
            
        }
        else
            // nothing to cumulate 
            break;
    }
    // end cumulate deallocated spaces
}*/


/**
 *
 */
bool Delete(void* pointer) 
{
#ifdef DM_HANDLER_MALLOC
    if (pointer == null) 
        return false;
    free(pointer);
    return true;
#else
    if (pointer == null || // pointer is null
        _dmLastAllocated == 0xFFFF ||  // nothing has been allocated yet
        pointer < _memoryBuffer+_dmCellMarkSize || // pointer out of range of memory buffer
        pointer >= _memoryBuffer+PREALLOC_SIZE) // pointer out of range of memory buffer
    {

#ifdef DMH_DEBUG_VERBOSE
        _failureCount++;
#endif
        return false;
    }
        
    uint32 actualIndex = ((byte*)pointer - (byte*)_memoryBuffer - _dmCellMarkSize);// get the position of cell info according to the pointer
    
    if (actualIndex >= PREALLOC_SIZE) {
#ifdef DMH_DEBUG_VERBOSE
        _failureCount++;
#endif
        return false;
    }
    
    CellInfo_t* pCell = (CellInfo_t*)((uint32)pointer - _dmCellMarkSize); 
    
    
    if (!pCell->isAllocated) {
        // this might mean, that the pointer points to improper place
        // or that the cell is already deallocated
        
#ifdef DMH_DEBUG_VERBOSE
        _failureCount++;
#endif
        
        //actualIndex = (uint32)pCell + actualIndex;
        //actualIndex *= 10;
        return true;
    }
    
    // actual deallocation   
    pCell->isAllocated = 0; // just mark the cell
    
    bool findLastAllocated = false;
    
    if (_dmLastAllocated == actualIndex) {
        // DO NOT DO THIS - after the last allocated, there could be still some other deallocated cells
        // pCell->size = 0; 
        findLastAllocated = true;
    }
    
    // pCell->size is the former size at the moment
#ifndef BOOTLOADER
    RemoveAllocationStatistic(pCell->size);
#endif
    
    // end of actual deallocation
      
    
    if (findLastAllocated) {
        //used is the _dmLastAllocated was deallocated
        actualIndex = 0; //_firstDeallocated != 0xFFFF ? _firstDeallocated : 0;
        uint32 formerLastAllocated = _dmLastAllocated;
        _dmLastAllocated = 0xFFFF;
        
        CellInfo_t* pOtherCell;
        
       
        while(actualIndex < PREALLOC_SIZE) {
            pOtherCell = (CellInfo_t*)(_memoryBuffer + actualIndex);
            if (pOtherCell->isAllocated)       
                if (_dmLastAllocated == 0xFFFF ||
                    actualIndex > _dmLastAllocated)
                    _dmLastAllocated = actualIndex;
            
            // DONT USE THIS RECOUNT FOR SOME REASONS IT PROVIDES INVALID VALUE
            //CellInfo_t* next = (CellInfo_t*)(pOtherCell+_dmCellMarkSize + pOtherCell->size);
            
            CellInfo_t* next = (CellInfo_t*)(_memoryBuffer+actualIndex+_dmCellMarkSize+pOtherCell->size);
            
            if (next >= pCell)
            {
                // we've matched the former _dmLastAllocated cell
                // count also with distorted cell marks
                break;
            }
            else {
                actualIndex+= _dmCellMarkSize + pOtherCell->size;
            }
        }
      
#ifdef DEBUG
        if (_dmLastAllocated > formerLastAllocated) {
            // this means the buffer got empty
            //x = 10*x*_dmLastAllocated*formerLastAllocated;
        }
#endif
        
        // only to confirm the change and to recreate endmarker
        //SetLastAllocated(_dmLastAllocated);
        
    }
    
    //SpaceOptimization(actualIndex);
    
    return true;
#endif    
}


#ifndef DM_HANDLER_MALLOC
    #ifndef BOOTLOADER
    uint32 AvailableSpace(void* pointer) 
    {
        uint32 ret = 0;
        if (null != pointer &&
            pointer >= _memoryBuffer+_dmCellMarkSize &&
            pointer < _memoryBuffer+PREALLOC_SIZE) 
        {
            CellInfo_t* pCell = (CellInfo_t*)((byte*)pointer-_dmCellMarkSize);
            if (pCell->isAllocated)
                ret = pCell->size;
        }
        
        return ret;
    }
    #endif
#endif
    
