using System;
using System.Linq;
using System.Collections.Generic;

public class RandomHelper
{
    private int _old;

    /// <summary>
    /// Função para gerar Lista de números inteiros aleatórios
    /// </summary>
    public static List<int> RandomList(int min, int max, int count)
    {
        return Enumerable.Range(min, max).OrderBy(x => Guid.NewGuid()).Take(count).ToList();
    }

    /// <summary>
    /// Função para gerar número inteiro aleatório (Sem Duplicatas)
    /// </summary>
    public int Range(int min, int max)
    {
        System.Random rnd = new System.Random();
        return _old = Enumerable.Range(min, max).OrderBy(x => rnd.Next()).Where(x => x != _old).Take(1).Single();
    }
}