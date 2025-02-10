/*using System;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json.Linq;

public class GeminiSpeechManager : MonoBehaviour
{
    private string geminiApiKey = "AIzaSyAHPtrJdFaT7u4AK4tjFEt1G3z-brywQ6I"; // Replace with your Gemini API Key
    private string ttsApiKey = "AIzaSyC0wxX3e5VFp0Fvfj3mgf19Rc8GKkr60Ws"; // Replace with your Google Cloud TTS API Key

    public TMP_InputField userInputField;  // Assign in Unity Inspector
    public TMP_Text responseText;  // Assign in Unity Inspector
    public AudioSource audioSource;  // Assign in Unity Inspector

    private string geminiUrl;
    private string ttsUrl = "https://texttospeech.googleapis.com/v1/text:synthesize";

    void Start()
    {
        geminiUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key=" + geminiApiKey;
    }

    // 🎯 Call this function on Button Click
    public void OnSendButtonClick()
    {
        string userInput = userInputField.text;
        StartCoroutine(SendRequestToGemini(userInput));
    }

    IEnumerator SendRequestToGemini(string userMessage)
    {
        string jsonData = "{ \"contents\": [ { \"role\": \"user\", \"parts\": [ { \"text\": \"" + userMessage + "\" } ] } ] }";

        using (UnityWebRequest request = new UnityWebRequest(geminiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("Gemini Response: " + response);

                string aiResponse = ExtractGeminiResponse(response);
                responseText.text = aiResponse;

                // 🔹 Convert AI response to speech
                StartCoroutine(ConvertTextToSpeech(aiResponse));
            }
            else
            {
                Debug.LogError("Error sending request: " + request.error);
                responseText.text = "Error: " + request.error;
            }
        }
    }

    string ExtractGeminiResponse(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "No response found";
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing AI response: " + ex.Message);
            return "Error parsing AI response";
        }
    }

    IEnumerator ConvertTextToSpeech(string text)
    {
        string jsonData = "{ \"input\": { \"text\": \"" + text + "\" }, \"voice\": { \"languageCode\": \"en-US\", \"name\": \"en-US-Wavenet-D\" }, \"audioConfig\": { \"audioEncoding\": \"MP3\" } }";

        string fullTtsUrl = ttsUrl + "?key=" + ttsApiKey;

        using (UnityWebRequest request = new UnityWebRequest(fullTtsUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("TTS Response: " + response);

                string audioBase64 = ExtractAudio(response);
                if (!string.IsNullOrEmpty(audioBase64))
                {
                    StartCoroutine(PlayAudio(audioBase64));
                }
                else
                {
                    Debug.LogError("TTS API returned an empty response.");
                }
            }
            else
            {
                Debug.LogError("Error converting text to speech: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    string ExtractAudio(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["audioContent"]?.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing TTS response: " + ex.Message);
            return "";
        }
    }

    IEnumerator PlayAudio(string base64Audio)
    {
        if (string.IsNullOrEmpty(base64Audio))
        {
            Debug.LogError("No audio data received.");
            yield break;
        }

        byte[] audioBytes = Convert.FromBase64String(base64Audio);
        string filePath = Application.persistentDataPath + "/tts_audio.mp3";
        File.WriteAllBytes(filePath, audioBytes);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to load audio: " + www.error);
            }
        }
    }
}*/




/*using System;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;  // Add this line to use UI components like Button
using TMPro;
using Newtonsoft.Json.Linq;

public class GeminiSpeechManager : MonoBehaviour
{
    private string geminiApiKey = "AIzaSyAHPtrJdFaT7u4AK4tjFEt1G3z-brywQ6I"; // Replace with your Gemini API Key
    private string ttsApiKey = "AIzaSyC0wxX3e5VFp0Fvfj3mgf19Rc8GKkr60Ws"; // Replace with your Google Cloud TTS API Key
    private string sttApiKey = "AIzaSyC0wxX3e5VFp0Fvfj3mgf19Rc8GKkr60Ws"; // Replace with your Google Cloud STT API Key

    private string geminiUrl;
    private string ttsUrl = "https://texttospeech.googleapis.com/v1/text:synthesize";
    private string sttUrl = "https://speech.googleapis.com/v1/speech:recognize";

    public TMP_InputField userInputField;  // Assign in Unity Inspector
    public TMP_Text responseText;  // Assign in Unity Inspector
    public AudioSource audioSource;  // Assign in Unity Inspector
    public Button recordButton;  // Assign in Unity Inspector

    private bool isRecording = false;
    private AudioClip recordedClip;

    void Start()
    {
        geminiUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key=" + geminiApiKey;
        ttsUrl += "?key=" + ttsApiKey;
        sttUrl += "?key=" + sttApiKey;

        recordButton.onClick.AddListener(StartRecording);
    }

    // 🎤 Start Recording
    public void StartRecording()
    {
        if (!isRecording)
        {
            Debug.Log("Recording started...");
            isRecording = true;
            recordedClip = Microphone.Start(null, false, 5, 44100);
            StartCoroutine(WaitForRecordingToFinish());
        }
    }

    // ⏹ Stop Recording after 5 seconds
    IEnumerator WaitForRecordingToFinish()
    {
        yield return new WaitForSeconds(5);
        StopRecording();
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            Debug.Log("Recording stopped...");
            isRecording = false;
            Microphone.End(null);

            StartCoroutine(ConvertSpeechToText());
        }
    }

    // 🗣 Convert Recorded Speech to Text
    IEnumerator ConvertSpeechToText()
    {
        Debug.Log("Converting speech to text...");
        
        byte[] audioData = WavUtility.FromAudioClip(recordedClip);
        string audioBase64 = Convert.ToBase64String(audioData);

        string jsonData = "{\"config\": {\"encoding\": \"LINEAR16\", \"sampleRateHertz\": 44100, \"languageCode\": \"en-US\"}, \"audio\": {\"content\": \"" + audioBase64 + "\"}}";

        using (UnityWebRequest request = new UnityWebRequest(sttUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("STT Response: " + response);

                string userText = ExtractTextFromSTT(response);
                if (!string.IsNullOrEmpty(userText))
                {
                    StartCoroutine(SendRequestToGemini(userText));
                }
            }
            else
            {
                Debug.LogError("Error in Speech-to-Text: " + request.error);
            }
        }
    }

    // 🔍 Extract Text from Speech-to-Text API Response
    string ExtractTextFromSTT(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["results"]?[0]?["alternatives"]?[0]?["transcript"]?.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing STT response: " + ex.Message);
            return "";
        }
    }

    // 📤 Send User's Speech (converted to text) to Gemini AI
    IEnumerator SendRequestToGemini(string userMessage)
    {
        string jsonData = "{ \"contents\": [ { \"role\": \"user\", \"parts\": [ { \"text\": \"" + userMessage + "\" } ] } ] }";

        using (UnityWebRequest request = new UnityWebRequest(geminiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("Gemini Response: " + response);

                string aiResponse = ExtractGeminiResponse(response);
                responseText.text = aiResponse;

                // 🔹 Convert AI response to speech
                StartCoroutine(ConvertTextToSpeech(aiResponse));
            }
            else
            {
                Debug.LogError("Error sending request: " + request.error);
                responseText.text = "Error: " + request.error;
            }
        }
    }

    // 🔍 Extract AI Response Text
    string ExtractGeminiResponse(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "No response found";
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing AI response: " + ex.Message);
            return "Error parsing AI response";
        }
    }

    // 🗣 Convert AI Text to Speech (TTS)
    IEnumerator ConvertTextToSpeech(string text)
    {
        string jsonData = "{ \"input\": { \"text\": \"" + text + "\" }, \"voice\": { \"languageCode\": \"en-US\", \"name\": \"en-US-Wavenet-D\" }, \"audioConfig\": { \"audioEncoding\": \"MP3\" } }";

        using (UnityWebRequest request = new UnityWebRequest(ttsUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("TTS Response: " + response);

                string audioBase64 = ExtractAudio(response);
                if (!string.IsNullOrEmpty(audioBase64))
                {
                    StartCoroutine(PlayAudio(audioBase64));
                }
                else
                {
                    Debug.LogError("TTS API returned an empty response.");
                }
            }
            else
            {
                Debug.LogError("Error converting text to speech: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    // 🔍 Extract Audio from TTS Response
    string ExtractAudio(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["audioContent"]?.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing TTS response: " + ex.Message);
            return "";
        }
    }

    // 🔊 Play AI-Generated Speech
    IEnumerator PlayAudio(string base64Audio)
    {
        if (string.IsNullOrEmpty(base64Audio))
        {
            Debug.LogError("No audio data received.");
            yield break;
        }

        byte[] audioBytes = Convert.FromBase64String(base64Audio);
        string filePath = Application.persistentDataPath + "/tts_audio.mp3";
        File.WriteAllBytes(filePath, audioBytes);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to load audio: " + www.error);
            }
        }
    }
}*/



/*

using System;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class GeminiSpeechManager : MonoBehaviour
{
    private string geminiApiKey = "AIzaSyDjMsTibCY95sKozNVk_CBR7WugwKG5I7Q"; // Replace with your Gemini API Key
    private string ttsApiKey = "AIzaSyDvqeRHwtCAni3f7uj3Om5Eq9hQa-m5WCg"; // Replace with your Google Cloud TTS API Key

    public Button recordButton;  // Assign in Unity Inspector
    public TMP_Text responseText;  // Assign in Unity Inspector
    public AudioSource audioSource;  // Assign in Unity Inspector

    private string geminiUrl;
    private string ttsUrl = "https://texttospeech.googleapis.com/v1/text:synthesize";

    private bool isRecording = false;
    private AudioClip recordedClip;

    void Start()
    {
        geminiUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key=" + geminiApiKey;
        recordButton.onClick.AddListener(StartRecording);
    }

    // 🎤 Start Recording
    public void StartRecording()
    {
        if (!isRecording)
        {
            Debug.Log("Recording started...");
            isRecording = true;
            recordedClip = Microphone.Start(null, false, 5, 44100); // Record for 5 seconds
            StartCoroutine(WaitForRecordingToFinish());
        }
    }

    // ⏹ Stop Recording after 5 seconds
    IEnumerator WaitForRecordingToFinish()
    {
        yield return new WaitForSeconds(5);
        StopRecording();
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            Debug.Log("Recording stopped...");
            isRecording = false;
            Microphone.End(null);

            StartCoroutine(ConvertSpeechToText());
        }
    }

    // 🗣 Convert Recorded Speech to Text
    IEnumerator ConvertSpeechToText()
    {
        Debug.Log("Converting speech to text...");
        
        byte[] audioData = WavUtility.FromAudioClip(recordedClip);
        string audioBase64 = Convert.ToBase64String(audioData);

        string jsonData = "{\"config\": {\"encoding\": \"LINEAR16\", \"sampleRateHertz\": 44100, \"languageCode\": \"en-US\"}, \"audio\": {\"content\": \"" + audioBase64 + "\"}}";

        using (UnityWebRequest request = new UnityWebRequest("https://speech.googleapis.com/v1/speech:recognize?key=" + ttsApiKey, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("STT Response: " + response);

                string userText = ExtractTextFromSTT(response);
                if (!string.IsNullOrEmpty(userText))
                {
                    StartCoroutine(SendRequestToGemini(userText));
                }
            }
            else
            {
                Debug.LogError("Error in Speech-to-Text: " + request.error);
            }
        }
    }

    // 🔍 Extract Text from Speech-to-Text API Response
    string ExtractTextFromSTT(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["results"]?[0]?["alternatives"]?[0]?["transcript"]?.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing STT response: " + ex.Message);
            return "";
        }
    }

    // 📤 Send User's Speech (converted to text) to Gemini AI
    IEnumerator SendRequestToGemini(string userMessage)
    {
        string jsonData = "{ \"contents\": [ { \"role\": \"user\", \"parts\": [ { \"text\": \"" + userMessage + "\" } ] } ] }";

        using (UnityWebRequest request = new UnityWebRequest(geminiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("Gemini Response: " + response);

                string aiResponse = ExtractGeminiResponse(response);
                responseText.text = aiResponse;

                // 🔹 Convert AI response to speech
                StartCoroutine(ConvertTextToSpeech(aiResponse));
            }
            else
            {
                Debug.LogError("Error sending request: " + request.error);
                responseText.text = "Error: " + request.error;
            }
        }
    }

    string ExtractGeminiResponse(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "No response found";
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing AI response: " + ex.Message);
            return "Error parsing AI response";
        }
    }

    // 🗣 Convert AI Text to Speech (TTS)
    IEnumerator ConvertTextToSpeech(string text)
    {
        string jsonData = "{ \"input\": { \"text\": \"" + text + "\" }, \"voice\": { \"languageCode\": \"en-US\", \"name\": \"en-US-Wavenet-D\" }, \"audioConfig\": { \"audioEncoding\": \"MP3\" } }";

        string fullTtsUrl = ttsUrl + "?key=" + ttsApiKey;

        using (UnityWebRequest request = new UnityWebRequest(fullTtsUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("TTS Response: " + response);

                string audioBase64 = ExtractAudio(response);
                if (!string.IsNullOrEmpty(audioBase64))
                {
                    StartCoroutine(PlayAudio(audioBase64));
                }
                else
                {
                    Debug.LogError("TTS API returned an empty response.");
                }
            }
            else
            {
                Debug.LogError("Error converting text to speech: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    string ExtractAudio(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["audioContent"]?.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing TTS response: " + ex.Message);
            return "";
        }
    }

    IEnumerator PlayAudio(string base64Audio)
    {
        if (string.IsNullOrEmpty(base64Audio))
        {
            Debug.LogError("No audio data received.");
            yield break;
        }

        byte[] audioBytes = Convert.FromBase64String(base64Audio);
        string filePath = Application.persistentDataPath + "/tts_audio.mp3";
        File.WriteAllBytes(filePath, audioBytes);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to load audio: " + www.error);
            }
        }
    }
}*/


using System;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class GeminiSpeechManager : MonoBehaviour
{
    private string geminiApiKey = "AIzaSyDjMsTibCY95sKozNVk_CBR7WugwKG5I7Q"; // Replace with your Gemini API Key
    private string ttsApiKey = "AIzaSyDvqeRHwtCAni3f7uj3Om5Eq9hQa-m5WCg"; // Replace with your Google Cloud TTS API Key

    public string userPrompt; // ✨ Set this in the Inspector instead of an Input Field
    public TMP_Text responseText;  // Assign in Unity Inspector
    public AudioSource audioSource;  // Assign in Unity Inspector
    public Button sendButton;  // Assign the UI Button in Unity Inspector

    private string geminiUrl;
    private string ttsUrl = "https://texttospeech.googleapis.com/v1/text:synthesize";

    void Start()
    {
        geminiUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key=" + geminiApiKey;
        
        // 🔹 Add Button Click Listener
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(OnSendButtonClick);
        }
    }

    // 🎯 Called when Button is Clicked
    public void OnSendButtonClick()
    {
        StartCoroutine(SendRequestToGemini(userPrompt)); // 🔹 Uses the string from the Inspector
    }

    IEnumerator SendRequestToGemini(string userMessage)
    {
        string jsonData = "{ \"contents\": [ { \"role\": \"user\", \"parts\": [ { \"text\": \"" + userMessage + "\" } ] } ] }";

        using (UnityWebRequest request = new UnityWebRequest(geminiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("Gemini Response: " + response);

                string aiResponse = ExtractGeminiResponse(response);
                responseText.text = aiResponse;

                // 🔹 Convert AI response to speech
                StartCoroutine(ConvertTextToSpeech(aiResponse));
            }
            else
            {
                Debug.LogError("Error sending request: " + request.error);
                responseText.text = "Error: " + request.error;
            }
        }
    }

    string ExtractGeminiResponse(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "No response found";
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing AI response: " + ex.Message);
            return "Error parsing AI response";
        }
    }

    IEnumerator ConvertTextToSpeech(string text)
    {
        string jsonData = "{ \"input\": { \"text\": \"" + text + "\" }, \"voice\": { \"languageCode\": \"en-US\", \"name\": \"en-US-Wavenet-D\" }, \"audioConfig\": { \"audioEncoding\": \"MP3\" } }";

        string fullTtsUrl = ttsUrl + "?key=" + ttsApiKey;

        using (UnityWebRequest request = new UnityWebRequest(fullTtsUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("TTS Response: " + response);

                string audioBase64 = ExtractAudio(response);
                if (!string.IsNullOrEmpty(audioBase64))
                {
                    StartCoroutine(PlayAudio(audioBase64));
                }
                else
                {
                    Debug.LogError("TTS API returned an empty response.");
                }
            }
            else
            {
                Debug.LogError("Error converting text to speech: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    string ExtractAudio(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["audioContent"]?.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing TTS response: " + ex.Message);
            return "";
        }
    }

    IEnumerator PlayAudio(string base64Audio)
    {
        if (string.IsNullOrEmpty(base64Audio))
        {
            Debug.LogError("No audio data received.");
            yield break;
        }

        byte[] audioBytes = Convert.FromBase64String(base64Audio);
        string filePath = Application.persistentDataPath + "/tts_audio.mp3";
        File.WriteAllBytes(filePath, audioBytes);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to load audio: " + www.error);
            }
        }
    }
}
