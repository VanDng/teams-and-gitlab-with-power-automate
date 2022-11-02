// Reference
//  https://learn.microsoft.com/en-us/connectors/custom-connectors/write-code

namespace Base
{
    public abstract class ScriptBase
    {
        // Context object
        public IScriptContext Context { get; set; }

        // CancellationToken for the execution
        public CancellationToken CancellationToken { get; }

        // Helper: Creates a StringContent object from the serialized JSON
        protected StringContent CreateJsonContent(string serializedJson)
        {
            return new StringContent(serializedJson);
        }

        // Abstract method for your code
        public abstract Task<HttpResponseMessage> ExecuteAsync();
    }
}