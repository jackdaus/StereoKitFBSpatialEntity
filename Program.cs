using StereoKit;
using StereoKit.Framework;
using System;

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

        // Some nice floor for when we are in VR
        Matrix floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
        Material floorMaterial = new Material("floor.hlsl");
        floorMaterial.Transparency = Transparency.Blend;

        // Some poses for our UI windows
        //Pose window1Pose = new Pose(-0.5f, 0, -0.3f, Quat.LookDir(1, 0, 1));
        Pose window2Pose = new Pose(0.2f, -0.1f, -0.5f, Quat.LookDir(-0.5f, 0, 1));
        Pose window1Pose = new Pose(window2Pose.position + Vec3.Up * 0.2f, window2Pose.orientation);
        
        Guid? selectedAnchorId = null;

        // Core application loop
        SK.Run(() => {
            if (SK.System.displayType == Display.Opaque && !passthroughStepper.Enabled)
                Mesh.Cube.Draw(floorMaterial, floorTransform);

            // Passthrough menu
            UI.WindowBegin("Passthrough Menu", ref window1Pose);
            if (passthroughStepper.Available)
            {
                if (UI.Button("toggle"))
                {
                    passthroughStepper.Enabled = !passthroughStepper.Enabled;
                }
                UI.Label($"Passthrough is {(passthroughStepper.Enabled ? "ON" : "OFF")}");
            }
            else
            {
                UI.Label("Passthrough is not available :(");
            }
            UI.WindowEnd();

            // Spatial Anchor Menu
            UI.WindowBegin("Spatial Anchor Menu", ref window2Pose, new Vec2(30,0) * U.cm);
            if (spatialEntityStepper.Available)
            {
                UI.Label("FB Spatial Entity EXT available!");
                if (UI.Button("Create Anchor"))
                {
                    // We will create the anchor at the location just in front of the window (and we'll adopt the UI window's orientation).
                    Vec3 anchorPosition = window2Pose.position + window2Pose.Forward * .05f + Vec3.Up * 0.1f;
                    Pose pose = new Pose(anchorPosition, window2Pose.orientation);

                    // We can optionally provide some callbacks for when the async operation either completes successfully or fails.
                    spatialEntityStepper.CreateAnchor(
                        pose,
                        (Guid newAnchorUuid) => Log.Info($"Async anchor creation success. New anchor created: Uuid:{newAnchorUuid}"),
                        () => Log.Info("Async anchor creation success failed :("));
                }

                UI.SameLine();
                
                if (UI.Button("Load All"))
                    spatialEntityStepper.LoadAllAnchors();

                UI.SameLine();

                if (UI.Button("Erase All"))
                    spatialEntityStepper.DeleteAllAnchors();

                // List all Anchors
                UI.HSeparator();
                UI.Label($"Anchors Loaded ({spatialEntityStepper.AnchorCount})");

                foreach (var anchor in spatialEntityStepper.Anchors)
                {
                    // Use a PushId to avoid button Id collisions
                    UI.PushId(anchor.Uuid.ToString());
                    UI.PanelBegin();
                    
                    if (UI.Button($"{anchor.Uuid.ToString().Substring(0,14)}..."))
                    {
                        // Unselect the anchor (if already selected) or select the anchor (if not already selected)
                        if (selectedAnchorId == anchor.Uuid)
                            selectedAnchorId = null;
                        else
                            selectedAnchorId = anchor.Uuid;
                    }
                    UI.SameLine();
                    // Button to delete the selected anchor
                    if (UI.Button("Delete"))
                    {
                        spatialEntityStepper.DeleteAnchor(anchor.Uuid);
                    }

                    if (selectedAnchorId == anchor.Uuid)
                    {
                        UI.Label("XrSpace: " + anchor.XrSpace);
                        UI.Label("Located: " + anchor.LocateSuccess);
                        UI.Label(anchor.Pose.ToString());
                    }
                    
                    UI.PanelEnd();
                    UI.PopId();
                }
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