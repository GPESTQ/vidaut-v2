using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ObjetoSalvo
{
    public string id;
    public float px, py, pz;
    public float rx, ry, rz, rw;
    public float sx, sy, sz;
    public bool ativo;
}

[System.Serializable]
public class SnapshotPasso
{
    public int etapa;
    public int passo;
    public List<ObjetoSalvo> objetos = new List<ObjetoSalvo>();
}

[System.Serializable]
public class EtapaSalva
{
    public bool desbloqueada;
    public bool concluida;
    public int ultimoPassoConcluido = -1;
    public List<bool> passosConcluidos = new List<bool>();
}

[System.Serializable]
public class SaveData
{
    public int etapaAtual;
    public int passoAtual;
    public List<EtapaSalva> etapas = new List<EtapaSalva>();
    public List<SnapshotPasso> snapshotsPorPasso = new List<SnapshotPasso>();
}

public static class SaveSystem
{
    private static string CaminhoArquivo =>
        Path.Combine(Application.persistentDataPath, "progresso.json");

    public static void Salvar(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(CaminhoArquivo, json);
        }
        catch (System.Exception) { }
    }

    public static SaveData Carregar()
    {
        try
        {
            if (!File.Exists(CaminhoArquivo))
                return null;
            string json = File.ReadAllText(CaminhoArquivo);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public static void Deletar()
    {
        try
        {
            if (File.Exists(CaminhoArquivo))
                File.Delete(CaminhoArquivo);
        }
        catch (System.Exception) { }
    }

    public static bool ExisteSave() => File.Exists(CaminhoArquivo);

    public static SnapshotPasso EncontrarSnapshot(SaveData save, int etapa, int passo)
    {
        if (save == null) return null;
        return save.snapshotsPorPasso.Find(s => s.etapa == etapa && s.passo == passo);
    }

    public static void GravarSnapshot(SaveData save, int etapa, int passo, List<ObjetoSalvo> objetos)
    {
        var existente = save.snapshotsPorPasso.Find(s => s.etapa == etapa && s.passo == passo);
        if (existente != null)
        {
            existente.objetos = objetos;
        }
        else
        {
            save.snapshotsPorPasso.Add(new SnapshotPasso
            {
                etapa   = etapa,
                passo   = passo,
                objetos = objetos
            });
        }
    }
}
