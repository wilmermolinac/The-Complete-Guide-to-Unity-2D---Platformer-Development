using UnityEngine;

/// <summary>
/// Clase que gestiona el audio global del juego, incluyendo música de fondo (BGM) y efectos de sonido (SFX).
/// Utiliza un patrón Singleton para garantizar una única instancia en toda la aplicación.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Instancia única de AudioManager.

    [Header("Audio Source")]
    [SerializeField] private AudioSource[] _sfx; // Arreglo de fuentes de audio para efectos de sonido (SFX).
    [SerializeField] private AudioSource[] _bgm; // Arreglo de fuentes de audio para música de fondo (BGM).
    private int _currentBgmIndex = 0; // Índice de la música de fondo que se está reproduciendo actualmente.

    /// <summary>
    /// Método llamado al inicializar el objeto. 
    /// Configura el patrón Singleton y asegura que este objeto no se destruya entre escenas.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // Evita que este objeto se destruya al cambiar de escena.

        // Configura el Singleton.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject); // Destruye instancias duplicadas.
        }
        
        //
        if (_bgm.Length <= 0)
        {
            return;
        }

        // Revisa periódicamente si hay música reproduciéndose.
        InvokeRepeating(nameof(PlayMusicIfNeeded), 0, 2);
    }

    /// <summary>
    /// Comprueba si hay música reproduciéndose. Si no, inicia una nueva.
    /// </summary>
    public void PlayMusicIfNeeded()
    {
        if (!_bgm[_currentBgmIndex].isPlaying)
        {
            PlayRandomBgm(); // Reproduce música de fondo aleatoria.
        }
    }

    /// <summary>
    /// Reproduce una pista de música de fondo seleccionada al azar.
    /// </summary>
    public void PlayRandomBgm()
    {
        _currentBgmIndex = Random.Range(0, _bgm.Length); // Selecciona un índice aleatorio.
        PlayBgm(_currentBgmIndex); // Reproduce la pista seleccionada.
    }

    /// <summary>
    /// Reproduce una pista de música de fondo específica según su índice.
    /// </summary>
    public void PlayBgm(int bgmIndex)
    {

        if (_bgm.Length <= 0)
        {
            Debug.LogWarning("You have no music on audio manager!");
            return;
        }
        
        // Detiene todas las pistas de música de fondo en reproducción.
        for (int i = 0; i < _bgm.Length; i++)
        {
            _bgm[i].Stop();
        }

        _currentBgmIndex = bgmIndex; // Actualiza el índice actual.
        _bgm[bgmIndex].Play(); // Reproduce la música de fondo seleccionada.
    }

    /// <summary>
    /// Reproduce un efecto de sonido específico según su índice. 
    /// Opcionalmente aplica un tono aleatorio.
    /// </summary>
    public void PlaySfx(int sfxIndex, bool isRandomPitch = true)
    {
        if (sfxIndex >= _sfx.Length)
        {
            return; // Verifica que el índice sea válido.
        }

        if (isRandomPitch)
        {
            _sfx[sfxIndex].pitch = Random.Range(0.8f, 1.2f); // Aplica un tono aleatorio.
        }

        _sfx[sfxIndex].Play(); // Reproduce el efecto de sonido.
    }

    /// <summary>
    /// Detiene la reproducción de un efecto de sonido específico.
    /// </summary>
    public void StopSfx(int sfxIndex)
    {
        _sfx[sfxIndex].Stop(); // Detiene el efecto de sonido.
    }
}