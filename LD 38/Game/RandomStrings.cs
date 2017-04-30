using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class RandomStrings
    {
        public static string RandomEarlNames()
        {
            string[] names = { "Graham", "Toler", "Acheson", "Trench" };
            int index = GameManager.Instance.RNG.Next(names.Length);
            return names[index];
        }

        public static string RandomViscountNames()
        {
            string[] names = { "MacCarty", "Parsons", "Needham", "Annesley" };
            int index = GameManager.Instance.RNG.Next(names.Length);
            return names[index];
        }

        public static string RandomBaronNames()
        {
            string[] names = { "Stewart", "Stuart", "Pery", "Knox", "Hare", "Newport", "Rous", "Campbell", "Anson", "Lee", "Pelham", "Noel" };
            int index = GameManager.Instance.RNG.Next(names.Length);
            return names[index];
        }

        public static string RandomImportantNames()
        {
            string[] names = { "Allen", "Davis", "Jackson", "Morris", "Thompson", "Baker", "Edwards", "James", "Parker",
                "Turner", "Bennett", "Evans", "Johnson", "Phillips", "Walker", "Brown", "Green", "Jones", "Price", "Ward",
                "Carter", "Griffiths", "King", "Roberts", "Watson", "Clark", "Hall", "Lee", "Robinson", "White", "Clarke",
                "Harris", "Lewis", "Shaw", "Williams", "Cook", "Harrison", "Martin", "Smith", "Wilson", "Cooper", "Hill",
                "Moore", "Taylor", "Wood", "Davies", "Hughes", "Morgan", "Thomas", "Wright" };
            int index = GameManager.Instance.RNG.Next(names.Length);
            return names[index];
        }

        public static string RandomFirstMale()
        {
            string[] names = { "Abraham",
"Alfred",
"Archie",
"Arnold",
"Arthur",
"Augustus",
"Baxter",
"Bernard",
"Bert",
"Bram",
"Cassius",
"Charley",
"Clarence",
"Claude",
"Clifford",
"Douglas",
"Edgar",
"Edison",
"Edmund",
"Edwin",
"Elmer",
"Enoch",
"Ernest",
"Everett",
"Fletcher",
"Floyd",
"Frank",
"Franklin",
"Gilbert",
"Grover",
"Harold",
"Harvey",
"Henry",
"Hugh",
"Hugo",
"Ives",
"Ivor",
"Jack",
"Jerome",
"Jules",
"Larkin",
"Leo",
"Louis",
"Luther",
"Martin",
"Merritt",
"Oliver",
"Oscar",
"Otto",
"Phineas",
"Raymond",
"Silas",
"Sterling",
"Tesla",
"Thaddeus",
"Theodore",
"Victor",
"Warren",
"Watson",
"Wellington",
"Willie" };
            int index = GameManager.Instance.RNG.Next(names.Length);
            return names[index];
        }

        public static string RandomFirstFemale()
        {
            string[] names = { "Ada",
"Adelaide",
"Adelia",
"Agatha",
"Alexandra",
"Alice",
"Alma",
"Anne",
"Arabella",
"Audrey",
"Bertha",
"Beryl",
"Blanche",
"Briar",
"Catherine",
"Clara",
"Clementine",
"Cora",
"Della",
"Ebba",
"Edith",
"Effie",
"Eleanor",
"Eliza",
"Elizabeth",
"Elsie",
"Emily",
"Emma",
"Esther",
"Evie",
"Fannie",
"Flora",
"Florence",
"Frances",
"Harriet",
"Hazel",
"Henrietta",
"Ida",
"Isabella",
"Jane",
"Josephine",
"Josie",
"Kitty",
"Lilian",
"Lily",
"Lottie",
"Lucy",
"Luella",
"Mabel",
"Maggie",
"Maida",
"Maisie",
"Marjorie",
"Martha",
"Mary",
"Millie",
"Minnie",
"Nellie",
"Nora",
"Ottilie",
"Rayne",
"Rosie",
"Ruth",
"Sophronia",
"Sylvia",
"Tillie",
"Victoria",
"Vinnie",
"Viola",
"Violet",
"Winnie",
"Zadie" };
            int index = GameManager.Instance.RNG.Next(names.Length);
            return names[index];
        }

    }
}
