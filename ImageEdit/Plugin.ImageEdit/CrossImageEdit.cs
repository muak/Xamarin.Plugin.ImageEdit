using Plugin.ImageEdit.Abstractions;
using System;

namespace Plugin.ImageEdit
{
  /// <summary>
  /// Cross platform ImageEdit implemenations
  /// </summary>
  public class CrossImageEdit
  {
    static Lazy<IImageEdit> Implementation = new Lazy<IImageEdit>(() => CreateImageEdit(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IImageEdit Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static IImageEdit CreateImageEdit()
    {
#if NETSTANDARD2_0
        return null;
#else
        return new ImageEdit();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
