using ShooterGame2D;

public static class SlimeFactory
{
    private static List<Func<PointF, Slime>> slimeCreators = new List<Func<PointF, Slime>>();

    public static void Register(Func<PointF, Slime> creator)
    {
        slimeCreators.Add(creator);
    }

    public static Slime CreateRandom(PointF pos)
    {
        if (slimeCreators.Count == 0)
        {
            // default
            return new Green(pos);
        }

        var rand = new Random();
        var creator = slimeCreators[rand.Next(slimeCreators.Count)];
        return creator(pos);
    }
}
