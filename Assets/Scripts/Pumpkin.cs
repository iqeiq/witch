

public class Pumpkin : Enemy
{

    public const int maxhp = 5;
    
    protected override void InitEnemy()
    {
        hp = maxhp;
    }

}
