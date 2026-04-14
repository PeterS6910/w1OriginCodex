#include "Tasks.h"
#include "System\baseTypes.h"
#include "System\Timer\timer32.h"
#include "System\Timer\sysTick.h"
#include "System\watchdog.h"

#define MAX_TASK_COUNT 24

typedef struct {
    DVoid2Void task;
    uint32 priority; 
    uint32 counter;
} taskInfo_t;


taskInfo_t _taskList[MAX_TASK_COUNT];
volatile uint32 _taskCount = 0;

int32 Tasks_Register(DVoid2Void task,uint32 priority) 
{
    int32 ret = -1;
    if (null!=task &&
        priority >= 1) 
    {
        for(int i=0;i<MAX_TASK_COUNT;i++) {
            if (_taskList[i].task == task) {
                _taskList[i].priority = priority;
                _taskList[i].counter = 0;
                return 0;
            }
        }
    

        for (int i=0;i<MAX_TASK_COUNT;i++) {
            if (_taskList[i].task == null) 
            {
                 _taskList[i].task = task;
                 _taskList[i].priority = priority;
                 _taskList[i].counter = 0;
                 _taskCount++;
                 break;
            }
        }
        
        ret= 0;
   }
   
   return ret;
}

void Tasks_Unregister(DVoid2Void task) {
    if (null != task)
    {
    
        for(int i=0;i<MAX_TASK_COUNT;i++) {
            if (_taskList[i].task == task) {
                _taskList[i].task = null;
                _taskCount--;
            }
        }
    
        
    }
}

#ifndef BOOTLOADER
void Tasks_UnregisterAll() {   
    for(int i=0;i<MAX_TASK_COUNT;i++) {
        _taskList[i].task = null;
    }
}
#endif

void Tasks_IncrementCounter() {
    // Increment all counter for task scheduling
    for (int i = 0; i < MAX_TASK_COUNT; i++) 
    {
        if (_taskList[i].task != null)
            _taskList[i].counter++;
    }
}

void Tasks_Run() {
    //SysTick_RegisterCriticalTask(Tasks_IncrementCounter);
    
#ifdef DEBUG
    uint32 taskExecutionTime = 0;
#endif
    
    while (1) 
    {
        for(int t=0;t<MAX_TASK_COUNT;t++) {
            if (_taskList[t].task != null) 
                // INCLUDING PREEMPTION                
                if (_taskList[t].counter >= _taskList[t].priority) 
                {
#ifdef DEBUG
                    taskExecutionTime = SysTick_GetTickCount();        
#endif
                    
                    _taskList[t].counter = 0;
                    
                    // Execution of the task itself
                    _taskList[t].task();
                    
      
#ifdef DEBUG
                    taskExecutionTime = SysTick_GetElapsedTime(taskExecutionTime);
#endif
                    
                } 
        }
        
        Tasks_IncrementCounter();
        
        WD_Kick();
    }
}



