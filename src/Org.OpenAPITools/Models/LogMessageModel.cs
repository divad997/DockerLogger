using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Org.OpenAPITools.Models
{
    /// <summary>
    /// LogMessageModel used for recieving client log messages
    /// </summary>
    [DataContract]
    public partial class LogMessageModel : IEquatable<LogMessageModel>
    {
        /// <summary>
        /// Unix epoch timestamp
        /// </summary>
        /// <value>Unix epoch timestamp</value>
        [DataMember(Name = "log_date", EmitDefaultValue = true)]
        [Required]
        public decimal LogDate { get; set; }

        /// <summary>
        /// Name of the application that&#39;s logging the request
        /// </summary>
        /// <value>Name of the application that&#39;s logging the request</value>
        [DataMember(Name = "application", EmitDefaultValue = false)]
        [Required]
        public string Application { get; set; }

        /// <summary>
        /// The actual content of the log message
        /// </summary>
        /// <value>The actual content of the log message</value>
        [DataMember(Name = "message", EmitDefaultValue = false)]
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class LogMessageModel {\n");
            sb.Append("  LogDate: ").Append(LogDate).Append("\n");
            sb.Append("  Application: ").Append(Application).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((LogMessageModel)obj);
        }

        /// <summary>
        /// Returns true if LogMessageModelsInner instances are equal
        /// </summary>
        /// <param name="other">Instance of LogMessageModelsInner to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(LogMessageModel other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    LogDate == other.LogDate ||

                    LogDate.Equals(other.LogDate)
                ) &&
                (
                    Application == other.Application ||
                    Application != null &&
                    Application.Equals(other.Application)
                ) &&
                (
                    Message == other.Message ||
                    Message != null &&
                    Message.Equals(other.Message)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)

                hashCode = hashCode * 59 + LogDate.GetHashCode();
                if (Application != null)
                    hashCode = hashCode * 59 + Application.GetHashCode();
                if (Message != null)
                    hashCode = hashCode * 59 + Message.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(LogMessageModel left, LogMessageModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LogMessageModel left, LogMessageModel right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
