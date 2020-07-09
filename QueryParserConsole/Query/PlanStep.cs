using System;
using System.Collections.Generic;
using System.Text;


public class PlanStep : IPlanStep
{
    public Guid Id { get; set; }
    public int Level { get; set; }

    public PlanResult GetResult()
    {
        throw new NotImplementedException();
    }
}
