using System;
using System.Collections.Generic;
using System.Text;


public class SearchStep : IPlanStep
{
    #region Private Fields
    StatementPart _part;
    #endregion

    #region Public Properties
    public Guid Id { get; set; }
    public int Level { get; set; }
    public StatementPart Part => _part;
    #endregion

    #region Constructors
    public SearchStep() { }
    public SearchStep(StatementPart part)
    {
        _part = part;
    }
    #endregion

    #region Public Methods
    public PlanResult GetResult()
    {
        throw new NotImplementedException();
    }

    public void GetResultText()
    {
        Console.WriteLine($"Executing Search: {Part.TextWithWhiteSpace}");
    }
    #endregion
}
