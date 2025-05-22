namespace LaWash.IoT.Transversal;
public class ApiRoutes
{
    #region BaseRoutes
    public const string Base = "api/parking-spots";
    #endregion

    #region ParkingRoutes
    public const string PostOccupy = "/{id}/occupy";
    public const string PostFree = "/{id}/free";
    public const string GetParkingSpotsStatus = Base;
    public const string PostCreateSpot = Base;
    public const string DeleteSpot = "/{id}";
    public const string GetAllDevices = "/devices";
    #endregion
}

