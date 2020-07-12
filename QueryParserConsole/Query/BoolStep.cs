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
            var search = (InputOne as SearchStep);
            search.GetResultText();
        }
        if (InputOne is BoolStep)
        {
            var b = (InputOne as BoolStep);
            b.GetResultText();
        }

        if (InputTwo is SearchStep)
        {
            var search = (InputTwo as SearchStep);
            search.GetResultText();
        }
        if (InputOne is BoolStep)
        {
            var b = (InputTwo as BoolStep);
            b.GetResultText();
        }

        Console.WriteLine($"Combining Results with {Boolean}");

    }
    #endregion

    #region Private Methods
    #endregion
}
