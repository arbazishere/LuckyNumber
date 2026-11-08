namespace ArbazAminKhan.Enums.Datas
{
    public abstract class Enums_Data { }

    public enum TransformAxesSpace { None = -1, World, Self }

    [System.Flags]
    public enum TransformAxes3_Flags
    {
        position = 1 << 0,
        rotation = 1 << 1,
        scale = 1 << 2,
    }

    //[System.Flags]
    //public enum Axis3_Flags
    //{
    //    x = 1 << 0,
    //    y = 1 << 1,
    //    z = 1 << 2,
    //}

    [System.Flags]
    public enum Axis4_Flags
    {
        x = 1 << 0,
        y = 1 << 1,
        z = 1 << 2,
        w = 1 << 3,
    }

    #region Camera

    [System.Flags]
    public enum Lens_Camera_Data_Flags
    {
        //Projection = 1 << 2,
        //OrthographicSize = 1 << 4,
        FieldOfView = 1 << 8,
        //NearClipPlane = 1 << 12,
        //FarClipPlane = 1 << 14,
        //Dutch = 1 << 16,
        //ModeOverride = 1 << 18,
        //PhysicalProperties = 1 << 20,
        //OrthoFromCamera = 1 << 22,
        //PhysicalFromCamera = 1 << 24,
        //AspectFromCamera = 1 << 26,
        //SourceCamera = 1 << 28,

        NeverSelect_Flag = 1 << 31,
    }

    [System.Flags]
    public enum ThirdPerson_Camera_Data_Flags
    {
        //EntityId = 1 << 0,
        //LocalIdentfierInFile = 1 << 2,
        //Script = 1 << 4, 
        //VcamOwner = 1 << 6, 
        //Damping = 1 << 8, 
        //ShoulderOffset = 1 << 12, 
        //VerticalArmLength = 1 << 16, 
        //CameraSide = 1 << 20, 
        CameraDistance = 1 << 24,
        //AvoidObstacles = 1 << 28, 
        //PreviousFollowTargetPosition = 1 << 29, 
        //DampingCorrection = 1 << 30, 
        //CamPosCollisionCorrection = 1 << 31, 

        NeverSelect_Flag = 1 << 31,
    }

    #endregion
}
