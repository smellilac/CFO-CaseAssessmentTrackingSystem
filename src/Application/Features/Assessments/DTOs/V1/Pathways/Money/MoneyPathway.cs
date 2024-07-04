namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

public partial class MoneyPathway : PathwayBase
{

    public MoneyPathway()
    {
        Questions = 
        [
            C1 = new C1(),
            C2 = new C2(),
            C3 = new C3(),
            C4 = new C4(),
            C5 = new C5(),
            C6 = new C6(),
            C7 = new C7(),
            C8 = new C8(),
            C9 = new C9(),
        ];
    }

    public C1 C1 { get; private set; }
    public C2 C2 { get; private set; }
    
    public C3 C3 { get; private set; }
    
    public C4 C4 { get; private set; }
    
    public C5 C5 { get; private set; }
    
    public C6 C6 { get; private set; }
    
    public C7 C7 { get; private set; }
    public C8 C8 { get; private set; }
    
    public C9 C9 { get; private set; }
    
    
    public override string Title => "Money";
    public override double Constant => 0.76845;
    public override string Icon => CatsIcons.Money;
    
}