public class Winding
{
    public static Winding CLOCKWISE = new Winding("clockwise");
    public static Winding COUNTERCLOCKWISE = new Winding("counterclockwise");
    public static Winding NONE = new Winding("none");
    private string name;

    private Winding(string name)
    {
        this.name = name;
    }
}