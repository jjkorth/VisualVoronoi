public class LR
{
    public static LR LEFT = new LR("left");
    public static LR RIGHT = new LR("right");
    public string name;

    public LR(string name)
    {
        this.name = name;
    }

    public static LR Other(LR lr)
    {
        return (lr == LEFT ? RIGHT : LEFT);
    }
}