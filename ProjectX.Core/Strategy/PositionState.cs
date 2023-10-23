using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectX.Core.Strategy;

public enum PositionStatus { POSITION_NONE = 0, POSITION_LONG, POSITION_SHORT }

public abstract class ActivePositionState
{    
    public PositionStatus PositionStatus { get; protected set; }        
    public bool IsActive { get; protected set; }    
}

public class InactivePositionState : ActivePositionState
{
    public InactivePositionState()
    {
        PositionStatus = PositionStatus.POSITION_NONE;
        IsActive = false;   
    }    
}

public class LiveActivePositionState : ActivePositionState
{   
    public LiveActivePositionState(PositionStatus status)
    {        
        PositionStatus = status;
        IsActive = true;
    }    
}