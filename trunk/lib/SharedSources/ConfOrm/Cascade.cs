using System;

namespace ConfOrm
{
	/// <summary>
	/// Defines behavior of soft-cascade actions.
	/// </summary>
	/// <remarks>
	/// To check the content or to include/exclude values, from cascade, is strongly recommanded the usage of extensions methods defined in <see cref="Extensions"/>
	/// </remarks>
	/// <seealso cref="Extensions.Has"/>
	/// <seealso cref="Extensions.Include"/>
	/// <seealso cref="Extensions.Exclude"/>
	[Flags]
	public enum Cascade
	{
		None = 0,
		Persist= 2,
		Refresh= 4,
		Merge= 8,
		Remove= 16,
		Detach= 32,
		ReAttach= 64,
		DeleteOrphans = 128,
		All = 256,
	}
}