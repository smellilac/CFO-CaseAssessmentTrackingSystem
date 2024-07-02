namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;

public class RelationshipsPathway : PathwayBase
{
    public override string Title => "Relationships";
    public override double Constant => 0.68620;
    public override string Icon => CatsIcons.Relationships;
    protected override IEnumerable<double> GetScores()
    {
        yield return 0;
    }
}