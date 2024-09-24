using System;

public static class Utilities
{
    public static Coords[] AdjacentCoordinatesForAnEvenRow(Coords targetCoords)
    {
        return new[]
        {
            new Coords(targetCoords.X + 1, targetCoords.Y),
            new Coords(targetCoords.X - 1, targetCoords.Y),
            new Coords(targetCoords.X, targetCoords.Y - 1),
            new Coords(targetCoords.X - 1, targetCoords.Y - 1),
            new Coords(targetCoords.X - 1, targetCoords.Y + 1),
            new Coords(targetCoords.X, targetCoords.Y + 1),
        };
    }

    public static Coords[] AdjacentCoordinatesForAnOddRow(Coords targetCoords)
    {
        return new[]
        {
            new Coords(targetCoords.X + 1, targetCoords.Y),
            new Coords(targetCoords.X - 1, targetCoords.Y),
            new Coords(targetCoords.X + 1, targetCoords.Y - 1),
            new Coords(targetCoords.X, targetCoords.Y - 1),
            new Coords(targetCoords.X, targetCoords.Y + 1),
            new Coords(targetCoords.X + 1, targetCoords.Y + 1),
        };
    }

    public static bool BubbleOnTheRight(double directionInRadians) =>
        directionInRadians >= 0 && directionInRadians <= Math.PI / 6 ||
        directionInRadians <= 0 && directionInRadians >= -(Math.PI / 6);

    public static bool BubbleOnTheRightAndTop(double directionInRadians) =>
        directionInRadians >= Math.PI / 6 && directionInRadians <= Math.PI / 6 * 3;

    public static bool BubbleOnTheLeftAndTop(double directionInRadians) =>
        directionInRadians >= Math.PI / 6 * 3 && directionInRadians <= Math.PI / 6 * 5;

    public static bool BubbleOnTheLeft(double directionInRadians) =>
        directionInRadians >= Math.PI / 6 * 5 && directionInRadians <= Math.PI ||
        directionInRadians >= -Math.PI && directionInRadians <= -Math.PI / 6 * 5;

    public static bool BubbleOnTheLeftAndDown(double directionInRadians) =>
        directionInRadians >= -Math.PI / 6 * 5 && directionInRadians <= -Math.PI / 6 * 3;

    public static bool BubbleOnTheRightAndDown(double directionInRadians) =>
        directionInRadians >= -Math.PI / 6 * 3 && directionInRadians <= -(Math.PI / 6);
}