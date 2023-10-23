# About

(Work in Progress) 

This [StereoKit](https://stereokit.net) project implements the XR_FB_spatial_entity OpenXR extensions for the StereoKit library. This enables us to track/persist world-locked anchors on Meta Quest devices! This project is built with the [sk-multi](https://github.com/StereoKit/StereoKit.Templates) template to support cross-platform app development on either Android or Quest+Link platforms.

![Demo of spatial anchors](demo.gif)

This project implements the following OpenXR extensions:
- XR_FB_spatial_entity
- XR_FB_spatial_entity_storage
- XR_FB_spatial_entity_query

# How to Use in Your Project

Here are the steps on how to use the Meta spatial anchor extension in your StereoKit project.
1. Copy/paste the **SpatialEntity** directory from this repo into your StereoKit project. It contains all the code to implement the OpenXR extension for StereoKit.

2. Add the SpatialEntityFBExt to your main program file. Note: This code must be called BEFORE calling `SK.Initialize`.
	```csharp
	SpatialEntityFBExt spatialEntityStepper = SK.AddStepper(new SpatialEntityFBExt());
	```

3. (Optional) Display some UI to interface with the spatial entity system.
	```csharp
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
	```

4. (Optional) Visualize the spatial anchors by looping over them and drawing something!
	```csharp
	// Visualize all loaded spatial anchor
	foreach (var anchor in spatialEntityStepper.Anchors)
	{
		// Just draw a nice orange cube for the anchor pose
		Mesh.Cube.Draw(Material.Default, anchor.Pose.ToMatrix(0.1f), new Color(1, 0.5f, 0));
	}
	```

# Features not (yet?) implemented

I'd like to get these additional OpenXR extensions up and running as well, when I find the time.

- [ ] Implement XR_FB_spatial_entity_sharing
- [ ] Implement XR_FB_spatial_entity_container
- [ ] Implement XR_FB_spatial_entity_storage_batch
- [ ] Implement XR_FB_spatial_entity_user
- [ ] Implement XR_FB_scene