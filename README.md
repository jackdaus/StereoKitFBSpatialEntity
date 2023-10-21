# About

(WIP) This [StereoKit](https://stereokit.net) project implements the XR_FB_spatial_entity OpenXR extensions for the StereoKit library. This enables us to track/persist world-locked anchors on Meta Quest devices! This project is built with the [sk-multi](https://github.com/StereoKit/StereoKit.Templates) template to support cross-platform app development on either Android or Quest+Link platforms.

This project implements the following OpenXR extensions:
- XR_FB_spatial_entity
- XR_FB_spatial_entity_storage
- XR_FB_spatial_entity_query

# How to Use in Your Project

If you want to take this code and implement it in your StereoKit project, the steps are pretty straightforward. Here, the spatial entity extension is implemented with a StereoKit [IStepper](https://stereokit.net/Pages/StereoKit.Framework/IStepper.html). So all you need to do is:
1. Copy/paste the **SpatialEntity** directory from this repo into your StereoKit project.

2. Add the stepper (must be done BEFORE calling `SK.Initialize`).
	```csharp
	SpatialEntityFBExt spatialEntityStepper = SK.AddStepper(new SpatialEntityFBExt());
	```

3. (Optional) Display some UI to interface with the spatial entity system.
	```csharp
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

# Stuff I might do

- [ ] Feature: Delete individual anchors
- [ ] Implement XR_FB_spatial_entity_sharing

Other interesting OpenXR extensions to consider implementing:
- [ ] Implement XR_FB_scene
- [ ] Implement XR_FB_spatial_entity_container
- [ ] Implement XR_FB_spatial_entity_storage_batch
- [ ] Implement XR_FB_spatial_entity_user