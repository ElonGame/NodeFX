﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeFX;
using System;
using System.IO;

[CustomEditor(typeof(NodeFXEffect))]
public class NodeFXEditor : Editor {

	NodeFXEffect targetEffect;

	private FileSystemWatcher _fileSystemWatcher;
	private GUIStyle headerStyle = new GUIStyle();

	void OnEnable() {
		targetEffect = (NodeFXEffect)target;
		GenerateStyles();
	}

	public override void OnInspectorGUI() {
		GUIDrawHeader();

        targetEffect.effectDefinition = (TextAsset)EditorGUILayout.ObjectField("Effect Definition", targetEffect.effectDefinition, typeof(TextAsset),false);
		
        if (targetEffect.effectDefinition != null) {
            GUIDrawTopShelfButtons();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUIDrawRefreshToggles();

            //CheckForUpdates();
        }
    }

    private void GUIDrawHeader()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("", headerStyle, GUILayout.MinHeight(75), GUILayout.MaxWidth(250));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void GUIDrawRefreshToggles() {
		
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();

        targetEffect.refreshOnFocus = EditorGUILayout.Toggle("Refresh On Window Focus", targetEffect.refreshOnFocus);
        targetEffect.refreshOnFileChange = EditorGUILayout.Toggle("Refresh On File Change", targetEffect.refreshOnFileChange);

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
        
    }

    private void GUIDrawTopShelfButtons()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Refresh"),
                                GUILayout.MinHeight(30),
                                GUILayout.MaxWidth(400)
                                ))
        {
            OnButtonRefresh();
        }

        if (GUILayout.Button(new GUIContent("Open Editor"),
                                GUILayout.MinHeight(30),
                                GUILayout.MaxWidth(400)
                                ))
        {
            OnButtonOpenEditor();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void CheckForUpdates()
    {
		if (_fileSystemWatcher == null) {
			CreateFileWatcher();
		}

		if (targetEffect.refreshOnFileChange) {
				_fileSystemWatcher.EnableRaisingEvents = true;
		} else {
				_fileSystemWatcher.EnableRaisingEvents = false;
		}
    }

    private void CreateFileWatcher()
    {
        Debug.Log("CreateFileWatcher");
		_fileSystemWatcher = new FileSystemWatcher();
		targetEffect.Refresh();
        string folder = targetEffect.path.Replace(targetEffect.effectDefinition.name + ".xml", "");
        string folderPath = Application.dataPath + folder.Substring(6);
        Debug.Log(folderPath);
		_fileSystemWatcher.Path = folderPath;

		/* Watch for changes in LastAccess and LastWrite times, and 
		the renaming of files or directories. */
		_fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite 
		| NotifyFilters.FileName | NotifyFilters.DirectoryName;
		
		// Only watch xml files.
		_fileSystemWatcher.Filter = "*.xml";

		// Add event handlers.
		_fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
		_fileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
		_fileSystemWatcher.Deleted += new FileSystemEventHandler(OnChanged);
		_fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        targetEffect.Refresh();
    }

	private void OnApplicationFocus() {
		if (targetEffect.refreshOnFocus) {
			targetEffect.Refresh();
		}
	}

    private void OnButtonOpenEditor()
    {
        if (targetEffect.effectDefinition != null) {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
            //start.FileName = "C:/Program Files/Side Effects Software/Houdini 16.5.323/bin/houdinifx.exe";
            start.FileName = targetEffect.source;
            System.Diagnostics.Process.Start(start);
        }
    }

    private void OnButtonRefresh() {
		targetEffect.Refresh();
	}

	private void GenerateStyles() {
		headerStyle.alignment = TextAnchor.UpperCenter;
		headerStyle.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/nodefx_logo.png");
	}
} 
