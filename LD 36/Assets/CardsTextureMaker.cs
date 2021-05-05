using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CardGame
{
    class CardsTextureMaker
    {
        private static string FPaper = "FrontPaper";
        private static string FOrnMandatory = "FrontOrnamentalMandatory";
        private static string FNoAttack = "FrontAttackHide";
        private static string FNoHp = "FrontHpHide";
        private static string FPortrait = "Portrait";

        public static Texture2D CreateTexture(bool has_attack, bool has_hp)
        {
            Texture2D paper = Resources.Load("Cards/Textures/" + FPaper) as Texture2D;
            Texture2D ornamental_mandatory = Resources.Load("Cards/Textures/" + FOrnMandatory) as Texture2D;
            Texture2D no_attack = Resources.Load("Cards/Textures/" + FNoAttack) as Texture2D;
            Texture2D no_hp = Resources.Load("Cards/Textures/" + FNoHp) as Texture2D;

            Texture2D result = new Texture2D(paper.width, paper.height);

            Color32[] paper_pixels = paper.GetPixels32();
            Color32[] orn_pixels = ornamental_mandatory.GetPixels32();
            Color32[] no_attack_pixels = no_attack.GetPixels32();
            Color32[] no_hp_pixels = no_hp.GetPixels32();

            Color32[] result_pixels = paper_pixels;
            for(int i=0; i < result_pixels.Length; i++)
            {
                result_pixels[i] = Color32.Lerp(result_pixels[i], orn_pixels[i], orn_pixels[i].a);

                if(!has_attack)
                {
                    result_pixels[i] = Color32.Lerp(result_pixels[i], no_attack_pixels[i], no_attack_pixels[i].a);
                }
                
                if(!has_hp)
                {
                    result_pixels[i] = Color32.Lerp(result_pixels[i], no_hp_pixels[i], no_hp_pixels[i].a);
                }
                
            }


            result.SetPixels32(result_pixels);
            result.Apply();

            return result;
        }

        public static Texture2D CreatePortrait(CardDefinition definion)
        {
            Texture2D portrait = Resources.Load("Cards/Textures/" + FPortrait) as Texture2D;

            if (definion.PortraitTexture == "None")
            {
                return portrait;
            }

            Texture2D result = new Texture2D(portrait.width, portrait.height);
            Texture2D card = Resources.Load("Cards/Textures/" + definion.PortraitTexture) as Texture2D;

            Color32[] portrait_pixels = portrait.GetPixels32();
            Color32[] result_pixels = portrait_pixels;
            Color32[] card_pixels = card.GetPixels32();

            for (int i = 0; i < result_pixels.Length; i++)
            {
                result_pixels[i] = Color32.Lerp(result_pixels[i], card_pixels[i], card_pixels[i].a);
            }

            result.SetPixels32(result_pixels);
            result.Apply();

            return result;
        }
    }
}
