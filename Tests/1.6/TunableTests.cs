using Xunit;

namespace ToxinWarcasket.Tests;

// The settings sliders' pure arithmetic (Tunable): where a release lands (Snap, StoredStep), the
// stops of a banded row (Stops) and which stop a stored value sits on (Nearest). Pure functions
// over floats, so the suite needs no game context beyond loading the type.
public class TunableTests
{
    [Fact]
    public void Snap_LandsOnTheStepAndClamps()
    {
        Assert.Equal(22f, Tunable.Snap(22.9f, 0f, 50f, 2f, 1f));
        Assert.Equal(24f, Tunable.Snap(23.1f, 0f, 50f, 2f, 1f));
        Assert.Equal(50f, Tunable.Snap(80f, 0f, 50f, 2f, 1f));
        Assert.Equal(10f, Tunable.Snap(3f, 10f, 300f, 10f, 1f));
    }

    [Fact]
    public void StoredStep_RoundsToWholeShownUnitsWhenScaled()
    {
        Assert.Equal(2f, Tunable.StoredStep(2f, 1f));
        Assert.Equal(3f, Tunable.StoredStep(2f, 3f));
        Assert.Equal(2f, Tunable.StoredStep(2f, 2f));
        Assert.Equal(4f, Tunable.StoredStep(3f, 2f));
    }

    // The jet's aiming time bands: quarter seconds to two, halves to five, the minimum first and
    // the boundary a stop once.
    [Fact]
    public void Stops_FollowTheBands()
    {
        float[] stops = Tunable.Stops(0f, 5f, [(2f, 0.25f), (5f, 0.5f)]);
        Assert.Equal(0f, stops[0]);
        Assert.Equal(0.25f, stops[1]);
        Assert.Contains(2f, stops);
        Assert.Contains(2.5f, stops);
        Assert.DoesNotContain(2.25f, stops);
        Assert.Equal(5f, stops[^1]);
        Assert.Equal(1 + 8 + 6, stops.Length);
    }

    // A minimum off its band's grid is the first stop: five, then the tens.
    [Fact]
    public void Stops_KeepAnOffGridMinimum()
    {
        float[] stops = Tunable.Stops(5f, 60f, [(60f, 10f)]);
        Assert.Equal(5f, stops[0]);
        Assert.Equal(10f, stops[1]);
        Assert.Equal(60f, stops[^1]);
        Assert.Equal(7, stops.Length);
    }

    [Fact]
    public void Nearest_PicksTheClosestStopLowerOnATie()
    {
        float[] stops = [0f, 1f, 2f, 4f];
        Assert.Equal(0, Tunable.Nearest(stops, -3f));
        Assert.Equal(2, Tunable.Nearest(stops, 2.4f));
        Assert.Equal(2, Tunable.Nearest(stops, 3f));
        Assert.Equal(3, Tunable.Nearest(stops, 9f));
    }
}
