public class MathRacing
{
    
    static Random rng = new Random();
    public static int GetNumber(int stat)
    {
        int[] valeurs = { -3, -2, -1, 0, 1, 2, 3 };
        double[] poids = new double[valeurs.Length];

        double bias = (stat - 70) / 50.0; // -1 à +1
        double intensite = 1; // à ajuster !

        double total = 0;

        for (int i = 0; i < valeurs.Length; i++)
        {
            poids[i] = Math.Exp(valeurs[i] * bias * intensite);
            total += poids[i];
        }

        // Tirage pondéré
        double tirage = rng.NextDouble() * total;
        double cumul = 0;

        for (int i = 0; i < valeurs.Length; i++)
        {
            cumul += poids[i];
            if (tirage <= cumul)
                return valeurs[i];
        }

        return -3; // sécurité
    }
}