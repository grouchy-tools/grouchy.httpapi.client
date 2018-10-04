properties {
   $VersionSuffix = $null
   $BasePath = Resolve-Path ..
   $SrcPath = "$BasePath\src"
   $ArtifactsPath = "$BasePath\artifacts"
   $ProjectPath = "$SrcPath\Burble\Burble.csproj"
   $TestProjectPath = "$SrcPath\Burble.Tests\Burble.Tests.csproj"
   $Configuration = if ($Configuration) {$Configuration} else { "Debug" }
}

task default -depends Clean, Build, Test, Package

task Clean {
   if (Test-Path -path $ArtifactsPath)
   {
      Remove-Item -path $ArtifactsPath -Recurse -Force | Out-Null
   }

   New-Item -Path $ArtifactsPath -ItemType Directory
}

task Build {
   exec { dotnet --version }
   exec { dotnet restore $ProjectPath }

   if ($VersionSuffix -eq $null -or $VersionSuffix -eq "") {
      exec { dotnet build $ProjectPath -c $Configuration -f netstandard2.0 --no-incremental }
      exec { dotnet build $ProjectPath -c $Configuration -f net451 --no-incremental }
   }
   else {
      exec { dotnet build $ProjectPath -c $Configuration -f netstandard2.0 --no-incremental --version-suffix $VersionSuffix }
      exec { dotnet build $ProjectPath -c $Configuration -f net451 --no-incremental --version-suffix $VersionSuffix }
   }
}

task Test -depends Build {
   exec { dotnet restore $TestProjectPath }
   exec { dotnet test $TestProjectPath -c $Configuration -f netcoreapp2.0 --filter Category!=local-only }
   exec { dotnet test $TestProjectPath -c $Configuration -f net451 --filter Category!=local-only }
}

task Package -depends Build {
   if ($VersionSuffix -eq $null -or $VersionSuffix -eq "") {
      exec { dotnet pack $ProjectPath -c $Configuration -o $ArtifactsPath }
   }
   else {
      exec { dotnet pack $ProjectPath -c $Configuration -o $ArtifactsPath --version-suffix $VersionSuffix }
   }
}
