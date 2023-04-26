namespace FatFamilyHelper.Helpers;

public interface IPingThrottler
{
    bool TryCanPing();
}
