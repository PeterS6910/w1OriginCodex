#ifndef BOOTLOADER

#include "testMode.h"

#include "Communication\Clsp485\clsp485.h"
#include "CardReaders\CrCommunicator.h"
#include "IO\Outputs.h"
#include "IO\Inputs.h"
#include "System\systemInfo.h"

int32 _previousKeyCode = -1;
int32 _passwordCorrect = 0;

bool _inTestMode = false;
bool _areOutputsInited = false;
bool _areOutputsOn = false;
bool _areInputsInited  = false;


void TestMode_EnterTestMenu(CardReader_t* cr) {
    if (cr == null || !cr->isOnline)
        return;
    _inTestMode = true;   
    _areOutputsOn = false;
    _areOutputsInited = false;
    _areInputsInited = false;
    
    CR_ClearDisplay(cr);
   
    char text[] = "TEST MODE I/0";
    
    CR_DisplayText(cr,0,0,text,13);
    
    char text2[] = "1:toggle outputs";
    CR_DisplayText(cr,0,1,text2,16);
    
    char text3[] = "2:bind inputs 0:exit";
    CR_DisplayText(cr,0,2,text3,20);
}

void UnsetInputsAndUnbindOutputs() {
     for(int32 i=0;i<SysInfo_GetOutputCount();i++) {
        Inputs_UnbindOutputToInput(i,0);
    }
    
    for(int32 i=0;i<SysInfo_GetInputCount();i++) {
        Inputs_UnsetInput(i);
    }
}

void TestMode_LeaveTestMenu(CardReader_t* cr) {
    if (cr == null || !cr->isOnline)
        return;
    _inTestMode = false;   
    
    CR_ClearDisplay(cr);
    
    if (_areOutputsInited && _areOutputsOn) {
        for(int32 i=0;i<SysInfo_GetOutputCount();i++) {
            Outputs_ActivateOutput(i,false);
        }
    }
    
    if (_areInputsInited) {
        UnsetInputsAndUnbindOutputs();       
    }
    
}


void TestMode_Test1() {
    if (!_areOutputsInited) {
        
        for(int32 i=0;i<SysInfo_GetOutputCount();i++) {
            Outputs_ConfigLevel(i,0,0,false);
        }
                            
        _areOutputsInited = true;
    }
    
    _areOutputsOn = !_areOutputsOn;
    
    for(int32 i=0;i<SysInfo_GetOutputCount();i++) {
        Outputs_ActivateOutput(i,_areOutputsOn);
    }
}

void TestMode_Test2() {
    if (!_areInputsInited) {
        for(int32 i=0;i<SysInfo_GetInputCount();i++) {
            Inputs_SetDiParameters(i,0,0,0, false);
        }
        
        for(int32 i=0;i<SysInfo_GetOutputCount();i++) {
            Inputs_BindOutputToInput(i,0);
        }

        _areInputsInited = true;
    }
    else {
        UnsetInputsAndUnbindOutputs();
        _areInputsInited = false;
    }
    
}

void TestMode_CRKeyPressed(CardReader_t* cr,uint32 key) 
{
    if (cr == null || !cr->isOnline)
        return;
    
    if (Clsp_IsAssigned() || Clsp_IsInConflict()) 
        return;
    
    if (_inTestMode) {
        switch(key) {
            case 0:
            case 0x0E: // No or cross button
                TestMode_LeaveTestMenu(cr);
                break;
            case 1:
                TestMode_Test1();
                break;
            case 2:
                TestMode_Test2();
                break;
                
        }
    }
    else {
    
        if (_previousKeyCode == -1) {
            if (key == 1) {
                _passwordCorrect++;
                _previousKeyCode = key;
            }
            else {
                _previousKeyCode = -1;
                _passwordCorrect = 0;
            }
            
            
        }
        else {
            switch(_previousKeyCode) {
                case 1:
                    if (key == 3) 
                    {
                        _passwordCorrect++;
                        _previousKeyCode = key;
                    }
                    else {
                        _passwordCorrect = 0;
                        _previousKeyCode = -1;
                    }
                    break;
                case 3:
                    if (key == 9) 
                    {
                        _passwordCorrect++;
                        _previousKeyCode = key;
                    }
                    else {
                        _passwordCorrect = 0;
                        _previousKeyCode = -1;
                    }
                    break;
                case 9:
                    if (key == 7) 
                    {
                        _passwordCorrect++;
                        
                        if (_passwordCorrect == 4) {
                            TestMode_EnterTestMenu(cr);
                        }
                    }
                    
                    _passwordCorrect = 0;
                    _previousKeyCode = -1;
                    break;
            }
        }
    }
    
}

void TestMode_CRInit()
{
    CrCommunicator_BindKeyPressed(TestMode_CRKeyPressed);
}

#endif // BOOTLOADER

