using KitchenLib.Customs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using KitchenDeconstructor;

namespace DeconstructorMod
{
    public class FontUtils

    {
        private static Dictionary<string, Font> m_FontIndex = new Dictionary<string, Font>();
        private static Dictionary<string, TMP_FontAsset> m_TMPFontIndex = new Dictionary<string, TMP_FontAsset>();


        private static List<GameObject> ListOfChildren = new List<GameObject>();
        /*
        public static void ApplyMaterial(GameObject prefab, string path, Material[] materials)
        {
            Transform transform = prefab.transform;
            string[] array = path.Split('/');
            string[] array2 = array;
            foreach (string n in array2)
            {
                transform = transform?.Find(n);
            }

            MeshRenderer meshRenderer = transform?.GetComponent<MeshRenderer>();
            if (!(meshRenderer == null))
            {
                meshRenderer.materials = materials;
            }
        }

        public static void ApplyMaterial<T>(GameObject prefab, string path, Material[] materials) where T : Renderer
        {
            Transform transform = prefab.transform;
            string[] array = path.Split('/');
            string[] array2 = array;
            foreach (string n in array2)
            {
                transform = transform?.Find(n);
            }

            T val = (((object)transform != null) ? transform.GetComponent<T>() : null);
            if (!((Object)val == (Object)null))
            {
                val.materials = materials;
            }
        }

        private static void getChildRecursive(GameObject obj)
        {
            if (null == obj || obj.name.ToLower().Contains("wallpaper") || obj.name.ToLower().Contains("flooring"))
            {
                return;
            }

            foreach (Transform item in obj.transform)
            {
                if (!(null == item))
                {
                    ListOfChildren.Add(item.gameObject);
                    getChildRecursive(item.gameObject);
                }
            }
        }
          */
        public static void SetupFontIndex()
        {
            if (m_FontIndex.Count > 0)
            {
                return;
            }

            Object[] array = Resources.FindObjectsOfTypeAll(typeof(Font));
            for (int i = 0; i < array.Length; i++)
            {
                Font font = (Font)array[i];
                if (!m_FontIndex.ContainsKey(font.name))
                {
                    m_FontIndex.Add(font.name, font);
                }
            }
            Object[] tmpArray = Resources.FindObjectsOfTypeAll(typeof(TMP_FontAsset));
            for (int i = 0; i < array.Length; i++)
            {
                TMP_FontAsset font = (TMP_FontAsset)tmpArray[i];
                Mod.LogWarning(font.name);
                if (!m_FontIndex.ContainsKey(font.name))
                {
                    m_TMPFontIndex.Add(font.name, font);
                }
            }
        }
        public static TMP_FontAsset GetExistingTMPFont(string pFontName)
        {
            if (m_TMPFontIndex.Count == 0) SetupFontIndex();
            if (m_TMPFontIndex.ContainsKey(pFontName))
            {
                return m_TMPFontIndex[pFontName];
            }

            return null;
        }

        public static Font GetExistingFont(string pFontName)
        {
            if (m_FontIndex.Count == 0) SetupFontIndex();
            if (m_FontIndex.ContainsKey(pFontName))
            {
                return m_FontIndex[pFontName];
            }

            return null;
        }
    }
}
