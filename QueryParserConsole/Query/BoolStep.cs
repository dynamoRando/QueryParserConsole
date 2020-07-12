using System;
using System.Collections.Generic;
using System.Text;


public class BoolStep : IPlanStep
{
    #region Public Properties
    public Guid Id { get; set; }
    public int Level { get; set; }
    public IPlanStep InputOne { get; set; }
    public IPlanStep InputTwo { get; set; }
    public string Boolean { get; set; }
    public string BoolStepTextWithWhiteSpace { get; set; }
    #endregion

    #region Constructors
    public BoolStep()
    {
        BoolStepTextWithWhiteSpace = string.Empty;
        Boolean = string.Empty;
    }
    #endregion

    #region Public Methods
    public PlanResult GetResult()
    {
        throw new NotImplementedException();
    }

    public void GetResultText()
    {
        if (InputOne is SearchStep)
        {
            var a = (InputOne as SearchStep);
            a.GetResultText();
        }
        if (InputOne is BoolStep)
        {
            var b = (InputOne as BoolStep);
            b.GetResultText();
        }

        if (InputTwo is SearchStep)
        {
            var c = (InputTwo as SearchStep);
            c.GetResultText();
        }
        if (InputTwo is BoolStep)
        {
            var d = (InputTwo as BoolStep);
            d.GetResultText();
        }

        Console.WriteLine($"Combining Results with {Boolean}");

    }
    #endregion

    #region Private Methods
    #endregion
}
