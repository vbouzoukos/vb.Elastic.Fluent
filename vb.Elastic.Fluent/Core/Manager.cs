using Nest;
using System;
using vb.Elastic.Fluent.Enums;

namespace vb.Elastic.Fluent.Core
{
    /// <summary>
    /// Elastic Search Manager 
    /// </summary>
    public class Manager
    {
        private static Manager _instance = null;
        private ElasticClient _client;
        /// <summary>
        /// Connection state with elasticsearch
        /// </summary>
        public EnConnectionStatus Status { get; set; }
        /// <summary>
        /// provate constructor for Singleton
        /// </summary>
        private Manager()
        {
            Status = EnConnectionStatus.NotSet;
        }
        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static Manager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Manager();
                }
                return _instance;
            }
        }
        /// <summary>
        /// Returns the NEST elastisearch client
        /// </summary>
        public static ElasticClient EsClient
        {
            get
            {
                if (Instance.Status != EnConnectionStatus.Connected)
                {
                    throw new Exception("There is no connection with the elastic search server.");
                }
                return Instance._client;
            }
        }
        /// <summary>
        /// Used as prefix to index name
        /// </summary>
        public string ApplicationName { get; set; }
        public string AppVersion { get; set; }

        /// <summary>
        /// Connects with elasticsearch service
        /// </summary>
        /// <param name="appVersion">Node connection url</param>
        /// <param name="applicationName">Installation Identifier</param>
        /// <param name="elasticUri">Node connection url</param>
        public void Connect(string appVersion, string applicationName, string elasticUri)
        {
            var uri = new Uri(elasticUri);
            var settings = new ConnectionSettings(uri);
            ApplicationName = applicationName;
            AppVersion = appVersion;
#if DEBUG
            settings.RequestTimeout(TimeSpan.FromSeconds(150));
            settings.DisableDirectStreaming(true);
#endif
            _client = new ElasticClient(settings);
            Status = EnConnectionStatus.Connected;
        }

        /// <summary>
        /// Connects with elasticsearch service
        /// </summary>
        /// <param name="appVersion">Node connection url</param>
        /// <param name="applicationName">Installation Identifier</param>
        /// <param name="settings">A connection settings instance</param>
        public void Connect(string appVersion, string applicationName, ConnectionSettings settings)
        {
            ApplicationName = applicationName;
            AppVersion = appVersion;
            _client = new ElasticClient(settings);
            Status = EnConnectionStatus.Connected;
        }
    }

}
