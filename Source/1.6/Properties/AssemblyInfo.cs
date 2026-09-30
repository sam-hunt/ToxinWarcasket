using System.Reflection;
using System.Runtime.InteropServices;

// Kept explicit (GenerateAssemblyInfo=false in the csproj) so the release flow can bump
// AssemblyVersion/AssemblyFileVersion in lockstep with About.xml's <modVersion> and
// CHANGELOG.md. Four-part X.Y.Z.0, where X.Y.Z is the mod's semantic version.
[assembly: AssemblyTitle("ToxinWarcasket")]
[assembly: AssemblyDescription("A tox gas warcasket set for Vanilla Factions Expanded - Pirates")]
[assembly: AssemblyProduct("ToxinWarcasket")]
[assembly: AssemblyCopyright("Copyright © 2026")]
[assembly: ComVisible(false)]
[assembly: Guid("b3d82aa4-5cce-4af5-a53e-b3fd06a974e7")]
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]
