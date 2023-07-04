using Nautilus.Utility;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using Nautilus.Assets;
using System.IO;

namespace Common.Mod
{
    internal class Utilities
    {
        /// <summary>
        ///     Creates the prefab info from information supplied.
        /// </summary>
        /// Credit to Ramune for this, check out his github https://github.com/RamuneNeptune

        public static PrefabInfo CreatePrefabInfo(string id, string name, string description, Atlas.Sprite sprite, int sizeX = 0, int sizeY = 0)
        {
            return PrefabInfo
                .WithTechType(id, name, description)
                .WithIcon(sprite)
                .WithSizeInInventory(new Vector2int(sizeX, sizeY));
        }

        public static PrefabInfo CreateBuildablePrefabInfo(string id, string name, string description, Atlas.Sprite sprite)
        {
            return PrefabInfo
                .WithTechType(id, name, description)
                .WithIcon(sprite);
        }

        /// <summary>
        /// Gets a sprite from the Assets folder by string
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>A <see cref="Texture2D"/></returns>
        /// Credit to Ramune for this, check out his github https://github.com/RamuneNeptune

        public static Texture2D GetTexture(string filename) => ImageUtils.LoadTextureFromFile(IOUtilities.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", filename + ".png"));

        /// <summary>
        /// Get a sprite from the game by TechType, or from the Assets folder by string
        /// </summary>
        /// <param name="FileOrTechType"></param>
        /// <returns>An <see cref="Atlas.Sprite"/></returns>
        /// <exception cref="ArgumentException"></exception>
        /// Credit to Ramune for this, check out his github https://github.com/RamuneNeptune

        public static Atlas.Sprite GetSprite(object FileOrTechType)
        {
            if (FileOrTechType is TechType techType) return SpriteManager.Get(techType);
            else if (FileOrTechType is string filename) return ImageUtils.LoadSpriteFromFile(IOUtilities.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", filename + ".png"));
            else throw new ArgumentException($"Incorrect type of '{FileOrTechType}' used in Sprite.Get()");
        }

        public static readonly Dictionary<TechType, TechType> curedCreatureList = new Dictionary<TechType, TechType>(TechTypeExtensions.sTechTypeComparer)
        {
            {
                TechType.Bladderfish,
                TechType.CuredBladderfish
            },
            {
                TechType.Boomerang,
                TechType.CuredBoomerang
            },
            {
                TechType.LavaBoomerang,
                TechType.CuredLavaBoomerang
            },
            {
                TechType.Eyeye,
                TechType.CuredEyeye
            },
            {
                TechType.LavaEyeye,
                TechType.CuredLavaEyeye
            },
            {
                TechType.GarryFish,
                TechType.CuredGarryFish
            },
            {
                TechType.HoleFish,
                TechType.CuredHoleFish
            },
            {
                TechType.Hoopfish,
                TechType.CuredHoopfish
            },
            {
                TechType.Hoverfish,
                TechType.CuredHoverfish
            },
            {
                TechType.Oculus,
                TechType.CuredOculus
            },
            {
                TechType.Peeper,
                TechType.CuredPeeper
            },
            {
                TechType.Reginald,
                TechType.CuredReginald
            },
            {
                TechType.Spadefish,
                TechType.CuredSpadefish
            },
            {
                TechType.Spinefish,
                TechType.CookedSpinefish
            }
        };
    }
}
