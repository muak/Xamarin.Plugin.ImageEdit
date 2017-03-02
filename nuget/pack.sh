#!/bin/sh
mono --runtime=v4.0 nuget.exe pack ImageEdit.Plugin.nuspec -symbols -Prop Configuration=Release -verbosity detailed -basepath ./ -OutputDirectory ~/projects/nuget
