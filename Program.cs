using NAudio.CoreAudioApi;

namespace simple_mic_mute;

static class Program
{
    private static readonly Icon iconActive = new("icons/mic_on.ico");
    private static readonly Icon iconMuted = new("icons/mic_off.ico");

    private static NotifyIcon? trayIcon;
    private static MMDevice? micDevice;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // 1. Inicializa o controle do microfone
        var enumerator = new MMDeviceEnumerator();
        micDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);

        // 2. Cria o ícone na Taskbar
        trayIcon = new NotifyIcon()
        {
            Icon = iconActive,
            Visible = true,
            Text = "Controle de Microfone"
        };

        // 3. Adiciona menu de contexto (Botão direito)
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Sair", null, (s, e) => Application.Exit());
        trayIcon.ContextMenuStrip = contextMenu;

        // 4. Clique no ícone faz o Toggle
        trayIcon.Click += (s, e) => {
            if (e is MouseEventArgs me && me.Button == MouseButtons.Left) {
                ToggleMic();
            }
        };

        UpdateIcon(); // Define o estado inicial

        Application.Run(); // Mantém o app rodando
    }

    private static void ToggleMic()
    {
        micDevice?.AudioEndpointVolume.Mute = !micDevice.AudioEndpointVolume.Mute;
        UpdateIcon();
        
        // Notificação do Windows
        string message = "O microfone não foi encontrado";
        ToolTipIcon icon = ToolTipIcon.Error;

        if (micDevice != null) {
            string status = micDevice.AudioEndpointVolume.Mute ? "Mutado" : "Ativo";
            message = $"O microfone agora está {status}";
            icon = ToolTipIcon.Info;
        }

        trayIcon?.ShowBalloonTip(2000, "Microfone", message, icon);
    }

    private static void UpdateIcon()
    {
        // Aqui você pode trocar o ícone para cores diferentes depois
        if (trayIcon != null && micDevice != null) {
            trayIcon.Icon = micDevice.AudioEndpointVolume.Mute ? iconMuted : iconActive;
            trayIcon.Text = micDevice.AudioEndpointVolume.Mute ? "Microfone: MUTADO" : "Microfone: ATIVO";
        }
    }  
}