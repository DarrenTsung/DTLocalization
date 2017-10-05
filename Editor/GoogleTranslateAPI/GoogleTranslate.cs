using System;
using System.Collections;
using UnityEngine;

using RestSharp.Contrib;

using DTLocalization.Internal;

namespace DTLocalization {
	public class GoogleTranslate {
		// PRAGMA MARK - Public Interface
		public GoogleTranslate(string clientEmail, byte[] privateKey) {
			requestFactory_ = new GoogleTranslateRequestFactory(clientEmail, privateKey);
		}

		public string Translate(string sourceText, string sourceLanguage, string targetLanguage) {
			if (sourceLanguage.Equals(targetLanguage)) {
				return sourceText;
			}

			var http = requestFactory_.CreateRequest();

			string requestJson = JsonUtility.ToJson(new GoogleTranslateRequest(sourceLanguage, targetLanguage, sourceText));
            var rawResponse = http.UploadString("https://translation.googleapis.com/language/translate/v2", requestJson);

			var response = JsonUtility.FromJson<GoogleTranslateResponse>(rawResponse);
			return HttpUtility.HtmlDecode(response.data.translations[0].translatedText);
		}


		// PRAGMA MARK - Internal
		private readonly GoogleTranslateRequestFactory requestFactory_;

		#pragma warning disable 0649
		[Serializable]
		private class GoogleTranslateRequest {
			public string source;
			public string target;
			public string[] q;

			public GoogleTranslateRequest(string sourceLanguage, string targetLanguage, string sourceText) {
				this.source = sourceLanguage;
				this.target = targetLanguage;
				this.q = new string[] { sourceText };
			}
		}

		[Serializable]
		private class GoogleTranslateResponse {
			public TranslateTextResponseList data;
		}

		[Serializable]
		private class TranslateTextResponseList {
			public TranslateTextResponseTranslation[] translations;
		}

		[Serializable]
		private class TranslateTextResponseTranslation {
			public string detectedSourceLanguage;
			public string model;
			public string translatedText;
		}
		#pragma warning restore 0649
	}
}