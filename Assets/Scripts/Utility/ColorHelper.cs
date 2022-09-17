using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEssentials.Extensions
{
    public class ColorHelper
    {
        public enum NamedColor
        {
            acid_green,
            adobe,
            algae,
            algae_green,
            almost_black,
            amber,
            amethyst,
            apple,
            apple_green,
            apricot,
            aqua,
            aqua_blue,
            aqua_green,
            aqua_marine,
            aquamarine,
            army_green,
            asparagus,
            aubergine,
            auburn,
            avocado,
            avocado_green,
            azul,
            azure,
            baby_blue,
            baby_green,
            baby_pink,
            baby_poo,
            baby_poop,
            baby_poop_green,
            baby_puke_green,
            baby_purple,
            baby_shit_brown,
            baby_shit_green,
            banana,
            banana_yellow,
            barbie_pink,
            barf_green,
            barney,
            barney_purple,
            battleship_grey,
            beige,
            berry,
            bile,
            black,
            bland,
            blood,
            blood_orange,
            blood_red,
            blue,
            blue_blue,
            blue_green,
            blue_grey,
            blue_purple,
            blue_touched_green,
            blue_touched_grey,
            blue_touched_purple,
            blue_violet,
            blue_with_a_hint_of_purple,
            blueberry,
            bluegreen,
            bluegrey,
            bluey_green,
            bluey_grey,
            bluey_purple,
            bluish,
            bluish_green,
            bluish_grey,
            bluish_purple,
            blurple,
            blush,
            blush_pink,
            booger,
            booger_green,
            bordeaux,
            boring_green,
            bottle_green,
            brick,
            brick_orange,
            brick_red,
            bright_aqua,
            bright_blue,
            bright_cyan,
            bright_green,
            bright_lavender,
            bright_light_blue,
            bright_light_green,
            bright_lilac,
            bright_lime,
            bright_lime_green,
            bright_magenta,
            bright_olive,
            bright_orange,
            bright_pink,
            bright_purple,
            bright_red,
            bright_sea_green,
            bright_sky_blue,
            bright_teal,
            bright_turquoise,
            bright_violet,
            bright_yellow,
            bright_yellow_green,
            british_racing_green,
            bronze,
            brown,
            brown_green,
            brown_grey,
            brown_orange,
            brown_red,
            brown_yellow,
            brownish,
            brownish_green,
            brownish_grey,
            brownish_orange,
            brownish_pink,
            brownish_purple,
            brownish_red,
            brownish_yellow,
            browny_green,
            browny_orange,
            bruise,
            bubble_gum_pink,
            bubblegum,
            bubblegum_pink,
            buff,
            burgundy,
            burnt_orange,
            burnt_red,
            burnt_siena,
            burnt_sienna,
            burnt_umber,
            burnt_yellow,
            burple,
            butter,
            butter_yellow,
            butterscotch,
            cadet_blue,
            camel,
            camo,
            camo_green,
            camouflage_green,
            canary,
            canary_yellow,
            candy_pink,
            caramel,
            carmine,
            carnation,
            carnation_pink,
            carolina_blue,
            celadon,
            celery,
            cement,
            cerise,
            cerulean,
            cerulean_blue,
            charcoal,
            charcoal_grey,
            chartreuse,
            cherry,
            cherry_red,
            chestnut,
            chocolate,
            chocolate_brown,
            cinnamon,
            claret,
            clay,
            clay_brown,
            clear_blue,
            cloudy_blue,
            cobalt,
            cobalt_blue,
            cocoa,
            coffee,
            cool_blue,
            cool_green,
            cool_grey,
            copper,
            coral,
            coral_pink,
            cornflower,
            cornflower_blue,
            cranberry,
            cream,
            creme,
            crimson,
            custard,
            cyan,
            dandelion,
            dark,
            dark_aqua,
            dark_aquamarine,
            dark_beige,
            dark_blue,
            dark_blue_green,
            dark_blue_grey,
            dark_brown,
            dark_coral,
            dark_cream,
            dark_cyan,
            dark_forest_green,
            dark_fuchsia,
            dark_gold,
            dark_grass_green,
            dark_green,
            dark_green_blue,
            dark_grey,
            dark_grey_blue,
            dark_hot_pink,
            dark_indigo,
            dark_khaki,
            dark_lavender,
            dark_lilac,
            dark_lime,
            dark_lime_green,
            dark_magenta,
            dark_maroon,
            dark_mauve,
            dark_mint,
            dark_mint_green,
            dark_mustard,
            dark_navy,
            dark_navy_blue,
            dark_olive,
            dark_olive_green,
            dark_orange,
            dark_pastel_green,
            dark_peach,
            dark_periwinkle,
            dark_pink,
            dark_plum,
            dark_purple,
            dark_red,
            dark_rose,
            dark_royal_blue,
            dark_sage,
            dark_salmon,
            dark_sand,
            dark_sea_green,
            dark_seafoam,
            dark_seafoam_green,
            dark_sky_blue,
            dark_slate_blue,
            dark_tan,
            dark_taupe,
            dark_teal,
            dark_turquoise,
            dark_violet,
            dark_yellow,
            dark_yellow_green,
            darkblue,
            darkgreen,
            darkish_blue,
            darkish_green,
            darkish_pink,
            darkish_purple,
            darkish_red,
            deep_aqua,
            deep_blue,
            deep_brown,
            deep_green,
            deep_lavender,
            deep_lilac,
            deep_magenta,
            deep_orange,
            deep_pink,
            deep_purple,
            deep_red,
            deep_rose,
            deep_sea_blue,
            deep_sky_blue,
            deep_teal,
            deep_turquoise,
            deep_violet,
            denim,
            denim_blue,
            desert,
            diarrhea,
            dirt,
            dirt_brown,
            dirty_blue,
            dirty_green,
            dirty_orange,
            dirty_pink,
            dirty_purple,
            dirty_yellow,
            dodger_blue,
            drab,
            drab_green,
            dried_blood,
            duck_egg_blue,
            dull_blue,
            dull_brown,
            dull_green,
            dull_orange,
            dull_pink,
            dull_purple,
            dull_red,
            dull_teal,
            dull_yellow,
            dusk,
            dusk_blue,
            dusky_blue,
            dusky_pink,
            dusky_purple,
            dusky_rose,
            dust,
            dusty_blue,
            dusty_green,
            dusty_lavender,
            dusty_orange,
            dusty_pink,
            dusty_purple,
            dusty_red,
            dusty_rose,
            dusty_teal,
            earth,
            easter_green,
            easter_purple,
            ecru,
            egg_shell,
            eggplant,
            eggplant_purple,
            eggshell,
            eggshell_blue,
            electric_blue,
            electric_green,
            electric_lime,
            electric_pink,
            electric_purple,
            emerald,
            emerald_green,
            evergreen,
            faded_blue,
            faded_green,
            faded_orange,
            faded_pink,
            faded_purple,
            faded_red,
            faded_yellow,
            fawn,
            fern,
            fern_green,
            fire_engine_red,
            flat_blue,
            flat_green,
            fluorescent_green,
            fluro_green,
            foam_green,
            forest,
            forest_green,
            forrest_green,
            french_blue,
            fresh_green,
            frog_green,
            fuchsia,
            gold,
            golden,
            golden_brown,
            golden_rod,
            golden_yellow,
            goldenrod,
            grape,
            grape_purple,
            grapefruit,
            grass,
            grass_green,
            grassy_green,
            green,
            green_apple,
            green_blue,
            green_brown,
            green_grey,
            green_teal,
            green_touched_blue,
            green_touched_yellow,
            green_yellow,
            greenblue,
            greenish,
            greenish_beige,
            greenish_blue,
            greenish_brown,
            greenish_cyan,
            greenish_grey,
            greenish_tan,
            greenish_teal,
            greenish_turquoise,
            greenish_yellow,
            greeny_blue,
            greeny_brown,
            greeny_grey,
            greeny_yellow,
            grey,
            grey_blue,
            grey_brown,
            grey_green,
            grey_pink,
            grey_purple,
            grey_teal,
            grey_touched_blue,
            grey_touched_green,
            greyblue,
            greyish,
            greyish_blue,
            greyish_brown,
            greyish_green,
            greyish_pink,
            greyish_purple,
            greyish_teal,
            gross_green,
            gunmetal,
            hazel,
            heather,
            heliotrope,
            highlighter_green,
            hospital_green,
            hot_green,
            hot_magenta,
            hot_pink,
            hot_purple,
            hunter_green,
            ice,
            ice_blue,
            icky_green,
            indian_red,
            indigo,
            indigo_blue,
            iris,
            irish_green,
            ivory,
            jade,
            jade_green,
            jungle_green,
            kelley_green,
            kelly_green,
            kermit_green,
            key_lime,
            khaki,
            khaki_green,
            kiwi,
            kiwi_green,
            lavender,
            lavender_blue,
            lavender_pink,
            lawn_green,
            leaf,
            leaf_green,
            leafy_green,
            leather,
            lemon,
            lemon_green,
            lemon_lime,
            lemon_yellow,
            lichen,
            light_aqua,
            light_aquamarine,
            light_beige,
            light_blue,
            light_blue_green,
            light_blue_grey,
            light_bluish_green,
            light_bright_green,
            light_brown,
            light_burgundy,
            light_cyan,
            light_eggplant,
            light_forest_green,
            light_gold,
            light_grass_green,
            light_green,
            light_green_blue,
            light_greenish_blue,
            light_grey,
            light_grey_blue,
            light_grey_green,
            light_indigo,
            light_khaki,
            light_lavendar,
            light_lavender,
            light_light_blue,
            light_light_green,
            light_lilac,
            light_lime,
            light_lime_green,
            light_magenta,
            light_maroon,
            light_mauve,
            light_mint,
            light_mint_green,
            light_moss_green,
            light_mustard,
            light_navy,
            light_navy_blue,
            light_neon_green,
            light_olive,
            light_olive_green,
            light_orange,
            light_pastel_green,
            light_pea_green,
            light_peach,
            light_periwinkle,
            light_pink,
            light_plum,
            light_purple,
            light_red,
            light_rose,
            light_royal_blue,
            light_sage,
            light_salmon,
            light_sea_green,
            light_seafoam,
            light_seafoam_green,
            light_sky_blue,
            light_tan,
            light_teal,
            light_turquoise,
            light_urple,
            light_violet,
            light_yellow,
            light_yellow_green,
            light_yellowish_green,
            lightblue,
            lighter_green,
            lighter_purple,
            lightgreen,
            lightish_blue,
            lightish_green,
            lightish_purple,
            lightish_red,
            lilac,
            liliac,
            lime,
            lime_green,
            lime_yellow,
            lipstick,
            lipstick_red,
            macaroni_and_cheese,
            magenta,
            mahogany,
            maize,
            mango,
            manilla,
            marigold,
            marine,
            marine_blue,
            maroon,
            mauve,
            medium_blue,
            medium_brown,
            medium_green,
            medium_grey,
            medium_pink,
            medium_purple,
            melon,
            merlot,
            metallic_blue,
            mid_blue,
            mid_green,
            midnight,
            midnight_blue,
            midnight_purple,
            military_green,
            milk_chocolate,
            mint,
            mint_green,
            minty_green,
            mocha,
            moss,
            moss_green,
            mossy_green,
            mud,
            mud_brown,
            mud_green,
            muddy_brown,
            muddy_green,
            muddy_yellow,
            mulberry,
            murky_green,
            mushroom,
            mustard,
            mustard_brown,
            mustard_green,
            mustard_yellow,
            muted_blue,
            muted_green,
            muted_pink,
            muted_purple,
            nasty_green,
            navy,
            navy_blue,
            navy_green,
            neon_blue,
            neon_green,
            neon_pink,
            neon_purple,
            neon_red,
            neon_yellow,
            nice_blue,
            night_blue,
            ocean,
            ocean_blue,
            ocean_green,
            ocher,
            ochre,
            ocre,
            off_blue,
            off_green,
            off_white,
            off_yellow,
            old_pink,
            old_rose,
            olive,
            olive_brown,
            olive_drab,
            olive_green,
            olive_yellow,
            orange,
            orange_brown,
            orange_pink,
            orange_red,
            orange_yellow,
            orangeish,
            orangered,
            orangey_brown,
            orangey_red,
            orangey_yellow,
            orangish,
            orangish_brown,
            orangish_red,
            orchid,
            pale,
            pale_aqua,
            pale_blue,
            pale_brown,
            pale_cyan,
            pale_gold,
            pale_green,
            pale_grey,
            pale_lavender,
            pale_light_green,
            pale_lilac,
            pale_lime,
            pale_lime_green,
            pale_magenta,
            pale_mauve,
            pale_olive,
            pale_olive_green,
            pale_orange,
            pale_peach,
            pale_pink,
            pale_purple,
            pale_red,
            pale_rose,
            pale_salmon,
            pale_sky_blue,
            pale_teal,
            pale_turquoise,
            pale_violet,
            pale_yellow,
            parchment,
            pastel_blue,
            pastel_green,
            pastel_orange,
            pastel_pink,
            pastel_purple,
            pastel_red,
            pastel_yellow,
            pea,
            pea_green,
            pea_soup,
            pea_soup_green,
            peach,
            peachy_pink,
            peacock_blue,
            pear,
            periwinkle,
            periwinkle_blue,
            perrywinkle,
            petrol,
            pig_pink,
            pine,
            pine_green,
            pink,
            pink_purple,
            pink_red,
            pink_touched_purple,
            pinkish,
            pinkish_brown,
            pinkish_grey,
            pinkish_orange,
            pinkish_purple,
            pinkish_red,
            pinkish_tan,
            pinky,
            pinky_purple,
            pinky_red,
            piss_yellow,
            pistachio,
            plum,
            plum_purple,
            poison_green,
            poo,
            poo_brown,
            poop,
            poop_brown,
            poop_green,
            powder_blue,
            powder_pink,
            primary_blue,
            prussian_blue,
            puce,
            puke,
            puke_brown,
            puke_green,
            puke_yellow,
            pumpkin,
            pumpkin_orange,
            pure_blue,
            purple,
            purple_blue,
            purple_brown,
            purple_grey,
            purple_pink,
            purple_red,
            purple_touched_blue,
            purple_touched_pink,
            purpleish,
            purpleish_blue,
            purpleish_pink,
            purpley,
            purpley_blue,
            purpley_grey,
            purpley_pink,
            purplish,
            purplish_blue,
            purplish_brown,
            purplish_grey,
            purplish_pink,
            purplish_red,
            purply,
            purply_blue,
            purply_pink,
            putty,
            racing_green,
            radioactive_green,
            raspberry,
            raw_sienna,
            raw_umber,
            really_light_blue,
            red,
            red_brown,
            red_orange,
            red_pink,
            red_purple,
            red_violet,
            red_wine,
            reddish,
            reddish_brown,
            reddish_grey,
            reddish_orange,
            reddish_pink,
            reddish_purple,
            reddy_brown,
            rich_blue,
            rich_purple,
            robin_egg_blue,
            robins_egg,
            robins_egg_blue,
            rosa,
            rose,
            rose_pink,
            rose_red,
            rosy_pink,
            rouge,
            royal,
            royal_blue,
            royal_purple,
            ruby,
            russet,
            rust,
            rust_brown,
            rust_orange,
            rust_red,
            rusty_orange,
            rusty_red,
            saffron,
            sage,
            sage_green,
            salmon,
            salmon_pink,
            sand,
            sand_brown,
            sand_yellow,
            sandstone,
            sandy,
            sandy_brown,
            sandy_yellow,
            sap_green,
            sapphire,
            scarlet,
            sea,
            sea_blue,
            sea_green,
            seafoam,
            seafoam_blue,
            seafoam_green,
            seaweed,
            seaweed_green,
            sepia,
            shamrock,
            shamrock_green,
            shit,
            shit_brown,
            shit_green,
            shocking_pink,
            sick_green,
            sickly_green,
            sickly_yellow,
            sienna,
            silver,
            sky,
            sky_blue,
            slate,
            slate_blue,
            slate_green,
            slate_grey,
            slime_green,
            snot,
            snot_green,
            soft_blue,
            soft_green,
            soft_pink,
            soft_purple,
            spearmint,
            spring_green,
            spruce,
            squash,
            steel,
            steel_blue,
            steel_grey,
            stone,
            stormy_blue,
            straw,
            strawberry,
            strong_blue,
            strong_pink,
            sun_yellow,
            sunflower,
            sunflower_yellow,
            sunny_yellow,
            sunshine_yellow,
            swamp,
            swamp_green,
            tan,
            tan_brown,
            tan_green,
            tangerine,
            taupe,
            tea,
            tea_green,
            teal,
            teal_blue,
            teal_green,
            tealish,
            tealish_green,
            terra_cotta,
            terracota,
            terracotta,
            tiffany_blue,
            tomato,
            tomato_red,
            topaz,
            toupe,
            toxic_green,
            tree_green,
            true_blue,
            true_green,
            turquoise,
            turquoise_blue,
            turquoise_green,
            turtle_green,
            twilight,
            twilight_blue,
            ugly_blue,
            ugly_brown,
            ugly_green,
            ugly_pink,
            ugly_purple,
            ugly_yellow,
            ultramarine,
            ultramarine_blue,
            umber,
            velvet,
            vermillion,
            very_dark_blue,
            very_dark_brown,
            very_dark_green,
            very_dark_purple,
            very_light_blue,
            very_light_brown,
            very_light_green,
            very_light_pink,
            very_light_purple,
            very_pale_blue,
            very_pale_green,
            vibrant_blue,
            vibrant_green,
            vibrant_purple,
            violet,
            violet_blue,
            violet_pink,
            violet_red,
            viridian,
            vivid_blue,
            vivid_green,
            vivid_purple,
            vomit,
            vomit_green,
            vomit_yellow,
            warm_blue,
            warm_brown,
            warm_grey,
            warm_pink,
            warm_purple,
            washed_out_green,
            water_blue,
            watermelon,
            weird_green,
            wheat,
            white,
            windows_blue,
            wine,
            wine_red,
            wintergreen,
            wisteria,
            yellow,
            yellow_brown,
            yellow_green,
            yellow_ochre,
            yellow_orange,
            yellow_tan,
            yellow_touched_green,
            yellowgreen,
            yellowish,
            yellowish_brown,
            yellowish_green,
            yellowish_orange,
            yellowish_tan,
            yellowy_brown,
            yellowy_green
        }

        private Dictionary<NamedColor, Color> _colors;

        private static ColorHelper _instance;
        public static ColorHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ColorHelper();
                }

                return _instance;
            }
        }

        private ColorHelper()
        {
            _colors = new Dictionary<NamedColor, Color>();

            using (var reader = new StreamReader(@"Assets\Database\colors.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (values.Count() != 4)
                    {
                        throw new System.Exception("Expected exactly 4 values per line in colors.csv file - 1 string for the color name, 3 ints for the RBG values in range 0-255.");
                    }

                    string colName = values[0];
                    NamedColor _color = (NamedColor) Enum.Parse(typeof(NamedColor), colName);
                    float r = float.Parse(values[1]) / 255f;
                    float g = float.Parse(values[2]) / 255f;
                    float b = float.Parse(values[3]) / 255f;

                    _colors[_color] = new Color(r, g, b);
                }
            }
        }

        /// <summary>
        /// from a color Name, find the appropriate color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public Color GetColor(NamedColor color)
        {
            if (_colors.ContainsKey(color))
            {
                return (_colors[color]);
            }

            Debugger.Log(0, "Utility", $"Defined color missing from colors list? Returning black for color {color}");
            return (Color.black);
        }

        /// <summary>
        /// from a given color, find the name of it
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public string FindColor(Color color)
        {
            float nearest = 99;
            string nameColor = "";
            Vector3 cin = new Vector3(color.r, color.g, color.b);

            foreach (KeyValuePair<NamedColor, Color> entry in _colors)
            {
                // do something with entry.Value or entry.Key
                Vector3 found = new Vector3(entry.Value.r, entry.Value.g, entry.Value.b);

                if (Vector3.Distance(found, cin) < nearest)
                {
                    nameColor = entry.Key.ToString();
                    nearest = Vector3.Distance(found, cin);
                }
            }
            return (nameColor);
        }
    }
}
