using StereoKit;
using StereoKit.Framework;

namespace StereoKitFBSpatialEntity;

class Program
{
    static void Main(string[] args)
    {
        // We must request these OpenXR extensions BEFORE we call SK.Initialize!
        PassthroughFBExt passthroughStepper     = SK.AddStepper(new PassthroughFBExt());
        SpatialEntityFBExt spatialEntityStepper = SK.AddStepper(new SpatialEntityFBExt());

        // Initialize StereoKit
        SKSettings settings = new SKSettings
        {
            appName = "StereoKitFBSpatialEntity",
            assetsFolder = "Assets",
        };
        if (!SK.Initialize(settings))
            return;

        // Create assets used by the app
        Pose cubePose = new Pose(0, 0, -0.5f);
        Model cube = Model.FromMesh(
            Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
            Material.UI);

        Matrix floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
        Material floorMaterial = new Material("floor.hlsl");
        floorMaterial.Transparency = Transparency.Blend;

        // Some poses for our UI windows
        Pose window1Pose = new Pose(-0.5f, 0, -0.3f, Quat.LookDir(1, 0, 1));
        Pose window2Pose = new Pose(0.2f, -0.1f, -0.3f, Quat.LookDir(-0.5f, 0, 1));

        // Core application loop
        SK.Run(() => {
            if (SK.System.displayType == Display.Opaque)
                Mesh.Cube.Draw(floorMaterial, floorTransform);

            UI.Handle("Cube", ref cubePose, cube.Bounds);
            cube.Draw(cubePose.ToMatrix());

            // Passthrough menu
            UI.WindowBegin("Passthrough Menu", ref window1Pose);
            if (passthroughStepper.Available)
            {

                if (UI.Button("toggle"))
                    passthroughStepper.Enabled = !passthroughStepper.Enabled;
                UI.Label($"Passthrough is {(passthroughStepper.Enabled ? "ON" : "OFF")}");
            }
            else
            {
                UI.Label("Passthrough is not available :(");
            }
            UI.WindowEnd();

            // Spatial Anchor Menu
            UI.WindowBegin("Spatial Anchor Menu", ref window2Pose);
            if (spatialEntityStepper.Available)
            {
                UI.Label("FB Spatial Entity EXT available!");
                if (UI.Button("Create Anchor"))
                {
                    // Create an anchor at pose of the right index finger tip
                    Pose fingerPose = Input.Hand(Handed.Right)[FingerId.Index, JointId.Tip].Pose;
                    spatialEntityStepper.CreateAnchor(fingerPose);
                }

                if (UI.Button("Load Anchors"))
                    spatialEntityStepper.LoadAllAnchors();

                if (UI.Button("Erase Anchors"))
                    spatialEntityStepper.EraseAllAnchors();
            }
            else
            {
                UI.Label("Spatial Anchor is not available :(");
            }
            UI.WindowEnd();

            // Visualize all loaded spatial anchor
            foreach (var anchor in spatialEntityStepper.Anchors)
            {
                // Just draw a nice orange cube for the anchor pose
                Mesh.Cube.Draw(Material.Default, anchor.Pose.ToMatrix(0.1f), new Color(1, 0.5f, 0));
            }
        });
    }
}