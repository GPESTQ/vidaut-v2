<h1>Vidaut V2</h1>

Projeto desenvolvido com **Unity** e **C#**.

## 🛠️ Tecnologias

- Unity
- C#
- Visual Studio Code

## 📋 Pré-requisitos

Antes de começar, instale:

1. **[Visual Studio Code](https://code.visualstudio.com/)**
2. **[Unity Hub](https://unity.com/download)** e o **Unity Editor** na versão do projeto
   - Ao instalar o Editor, marque o módulo **Windows Build Support** (Mono e/ou IL2CPP) e os módulos de outras plataformas que quiser usar
3. **[Git](https://git-scm.com/)**
4. **[.NET SDK](https://dotnet.microsoft.com/download)** (necessário para a extensão C# do VS Code)

> 💡 A versão exata do Unity está no arquivo `ProjectSettings/ProjectVersion.txt`. Use a mesma versão para evitar erros de compatibilidade.

## 🚀 Passo a passo para rodar o projeto

### 1. Clonar o repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd Vidaut-V2
```

### 2. Instalar a versão correta do Unity

1. Abra o **Unity Hub**
2. Vá em **Installs → Install Editor**
3. Escolha a versão indicada em `ProjectSettings/ProjectVersion.txt`
4. Em **Add modules**, marque **Windows Build Support** (e as outras plataformas desejadas)

### 3. Abrir o projeto na Unity

1. No Unity Hub, clique em **Add → Add project from disk**
2. Selecione a pasta do projeto clonado
3. Clique no projeto para abri-lo
4. Aguarde a importação dos assets (a primeira abertura pode demorar)

### 4. Configurar o VS Code como editor de scripts

1. Na Unity, vá em **Edit → Preferences → External Tools**
2. Em **External Script Editor**, selecione **Visual Studio Code**
3. Clique em **Regenerate project files**

### 5. Configurar o VS Code

Instale as extensões:

- **C# Dev Kit**
- **Unity** (Microsoft)

Depois, abra a pasta do projeto no VS Code (`File → Open Folder`) ou dê dois cliques em qualquer script `.cs` dentro da Unity.

### 6. Abrir a cena principal e testar

1. Na janela **Project**, abra `Assets/Scenes/<NOME_DA_CENA>.unity`
2. Clique no botão **Play** ▶️ para testar

## 📦 Gerar a build (Windows)

1. Vá em **File → Build Settings**
2. Selecione **Windows, Mac, Linux** (ou a plataforma desejada)
3. Confirme que as cenas estão em **Scenes In Build**
4. Clique em **Switch Platform** (se necessário)
5. Clique em **Build** e escolha a pasta de destino

## 📁 Estrutura do projeto

```
Assets/
├── Scenes/     # Cenas do jogo
├── Scripts/    # Scripts em C#
├── Prefabs/    # Prefabs
├── Sprites/    # Imagens e sprites
└── Audio/      # Sons e músicas
```

> Ajuste a estrutura acima conforme as pastas reais do projeto.

## 🤝 Fluxo de trabalho com Git

```bash
git checkout -b minha-feature     # criar uma branch
git add .
git commit -m "Descrição da alteração"
git push origin minha-feature
```

Depois, abra um **Pull Request** para a branch principal.

## ⚠️ Problemas comuns

| Problema | Solução |
|---|---|
| Erros de script no VS Code (sem autocomplete) | Na Unity: **Edit → Preferences → External Tools → Regenerate project files** |
| Projeto não abre / versão diferente | Instale a versão exata do Unity indicada em `ProjectVersion.txt` |
| Opção de build para Windows indisponível | Instale o módulo **Windows Build Support** pelo Unity Hub |
| Pastas `Library/` ou `Temp/` com erro | Feche a Unity, apague a pasta `Library/` e reabra o projeto |
