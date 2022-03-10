using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Zbx1425.DXDynamicTexture {

    public static partial class TextureManager {

        internal static partial class PostInstantiateTexturePatcher {

            class BveModelInfoClassWrapper {

                public object Src { get; }

                public BveModelInfoClassWrapper(object src) {
                    Src = src;
                    Type type = src.GetType();
                    FieldInfo[] fields = type.GetFields(DefaultBindingFlags);
                    MeshField = fields.First(f => f.FieldType == typeof(Mesh));
                    MaterialInfosField = fields.First(f => {
                        if (!f.FieldType.IsArray) return false;

                        var materialInfoTypeFields = f.FieldType.GetElementType().GetFields(DefaultBindingFlags);
                        bool hasMaterialTypeField = materialInfoTypeFields.Any(f2 => f2.FieldType == typeof(Material));
                        bool hasTextureTypeField = materialInfoTypeFields.Any(f2 => f2.FieldType == typeof(Texture));

                        return hasMaterialTypeField && hasTextureTypeField;
                    });
                }

                private FieldInfo MeshField;
                public Mesh Mesh {
                    get => (Mesh)MeshField.GetValue(Src);
                }

                private FieldInfo MaterialInfosField;
                public BveMaterialInfoClassWrapper[] MaterialInfos {
                    get => ((object[])MaterialInfosField.GetValue(Src))
                        .Select(material => new BveMaterialInfoClassWrapper(material))
                        .ToArray();
                }
            }

            class BveMaterialInfoClassWrapper {

                public object Src { get; }

                public BveMaterialInfoClassWrapper(object src) {
                    Src = src;
                    Type type = src.GetType();
                    FieldInfo[] fields = type.GetFields(DefaultBindingFlags);
                    MaterialField = fields.FirstOrDefault(f => f.FieldType == typeof(Material));
                    TextureField = fields.FirstOrDefault(f => f.FieldType == typeof(Texture));
                }

                private FieldInfo MaterialField;
                public Material Material {
                    get => (Material)MaterialField.GetValue(Src);
                    set => MaterialField.SetValue(Src, value);
                }

                private FieldInfo TextureField;
                public Texture Texture {
                    get => (Texture)TextureField.GetValue(Src);
                    set => TextureField.SetValue(Src, value);
                }
            }
        }
    }
}