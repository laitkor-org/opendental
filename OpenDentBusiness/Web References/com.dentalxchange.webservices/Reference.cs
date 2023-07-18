﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace OpenDentBusiness.com.dentalxchange.webservices {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="dciservice.svlSoapBinding", Namespace="http://www.dentalxchange.com/webservice")]
    public partial class WebServiceService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback lookupEligibilityOperationCompleted;
        
        private System.Threading.SendOrPostCallback lookupClaimStatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback lookupFamilyEligibilityOperationCompleted;
        
        private System.Threading.SendOrPostCallback lookupTerminalEligibilityOperationCompleted;
        
        private System.Threading.SendOrPostCallback lookupTerminalClaimStatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback updateTerminalOperationCompleted;
        
        private System.Threading.SendOrPostCallback lookupClaimOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public WebServiceService() {
            this.Url = "https://webservices.dentalxchange.com/dws/services/dciservice.svl";
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event lookupEligibilityCompletedEventHandler lookupEligibilityCompleted;
        
        /// <remarks/>
        public event lookupClaimStatusCompletedEventHandler lookupClaimStatusCompleted;
        
        /// <remarks/>
        public event lookupFamilyEligibilityCompletedEventHandler lookupFamilyEligibilityCompleted;
        
        /// <remarks/>
        public event lookupTerminalEligibilityCompletedEventHandler lookupTerminalEligibilityCompleted;
        
        /// <remarks/>
        public event lookupTerminalClaimStatusCompletedEventHandler lookupTerminalClaimStatusCompleted;
        
        /// <remarks/>
        public event updateTerminalCompletedEventHandler updateTerminalCompleted;
        
        /// <remarks/>
        public event lookupClaimCompletedEventHandler lookupClaimCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.dentalxchange.com/webservice", ResponseNamespace="http://www.dentalxchange.com/webservice")]
        [return: System.Xml.Serialization.SoapElementAttribute("lookupEligibilityReturn")]
        public Response lookupEligibility(Credentials in0, Request in1) {
            object[] results = this.Invoke("lookupEligibility", new object[] {
                        in0,
                        in1});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void lookupEligibilityAsync(Credentials in0, Request in1) {
            this.lookupEligibilityAsync(in0, in1, null);
        }
        
        /// <remarks/>
        public void lookupEligibilityAsync(Credentials in0, Request in1, object userState) {
            if ((this.lookupEligibilityOperationCompleted == null)) {
                this.lookupEligibilityOperationCompleted = new System.Threading.SendOrPostCallback(this.OnlookupEligibilityOperationCompleted);
            }
            this.InvokeAsync("lookupEligibility", new object[] {
                        in0,
                        in1}, this.lookupEligibilityOperationCompleted, userState);
        }
        
        private void OnlookupEligibilityOperationCompleted(object arg) {
            if ((this.lookupEligibilityCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.lookupEligibilityCompleted(this, new lookupEligibilityCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.dentalxchange.com/webservice", ResponseNamespace="http://www.dentalxchange.com/webservice")]
        [return: System.Xml.Serialization.SoapElementAttribute("lookupClaimStatusReturn")]
        public Response lookupClaimStatus(Credentials in0, Request in1) {
            object[] results = this.Invoke("lookupClaimStatus", new object[] {
                        in0,
                        in1});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void lookupClaimStatusAsync(Credentials in0, Request in1) {
            this.lookupClaimStatusAsync(in0, in1, null);
        }
        
        /// <remarks/>
        public void lookupClaimStatusAsync(Credentials in0, Request in1, object userState) {
            if ((this.lookupClaimStatusOperationCompleted == null)) {
                this.lookupClaimStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnlookupClaimStatusOperationCompleted);
            }
            this.InvokeAsync("lookupClaimStatus", new object[] {
                        in0,
                        in1}, this.lookupClaimStatusOperationCompleted, userState);
        }
        
        private void OnlookupClaimStatusOperationCompleted(object arg) {
            if ((this.lookupClaimStatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.lookupClaimStatusCompleted(this, new lookupClaimStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.dentalxchange.com/webservice", ResponseNamespace="http://www.dentalxchange.com/webservice")]
        [return: System.Xml.Serialization.SoapElementAttribute("lookupFamilyEligibilityReturn")]
        public Response lookupFamilyEligibility(Credentials in0, Request in1) {
            object[] results = this.Invoke("lookupFamilyEligibility", new object[] {
                        in0,
                        in1});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void lookupFamilyEligibilityAsync(Credentials in0, Request in1) {
            this.lookupFamilyEligibilityAsync(in0, in1, null);
        }
        
        /// <remarks/>
        public void lookupFamilyEligibilityAsync(Credentials in0, Request in1, object userState) {
            if ((this.lookupFamilyEligibilityOperationCompleted == null)) {
                this.lookupFamilyEligibilityOperationCompleted = new System.Threading.SendOrPostCallback(this.OnlookupFamilyEligibilityOperationCompleted);
            }
            this.InvokeAsync("lookupFamilyEligibility", new object[] {
                        in0,
                        in1}, this.lookupFamilyEligibilityOperationCompleted, userState);
        }
        
        private void OnlookupFamilyEligibilityOperationCompleted(object arg) {
            if ((this.lookupFamilyEligibilityCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.lookupFamilyEligibilityCompleted(this, new lookupFamilyEligibilityCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.dentalxchange.com/webservice", ResponseNamespace="http://www.dentalxchange.com/webservice")]
        [return: System.Xml.Serialization.SoapElementAttribute("lookupTerminalEligibilityReturn")]
        public Response lookupTerminalEligibility(Credentials in0, Request in1) {
            object[] results = this.Invoke("lookupTerminalEligibility", new object[] {
                        in0,
                        in1});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void lookupTerminalEligibilityAsync(Credentials in0, Request in1) {
            this.lookupTerminalEligibilityAsync(in0, in1, null);
        }
        
        /// <remarks/>
        public void lookupTerminalEligibilityAsync(Credentials in0, Request in1, object userState) {
            if ((this.lookupTerminalEligibilityOperationCompleted == null)) {
                this.lookupTerminalEligibilityOperationCompleted = new System.Threading.SendOrPostCallback(this.OnlookupTerminalEligibilityOperationCompleted);
            }
            this.InvokeAsync("lookupTerminalEligibility", new object[] {
                        in0,
                        in1}, this.lookupTerminalEligibilityOperationCompleted, userState);
        }
        
        private void OnlookupTerminalEligibilityOperationCompleted(object arg) {
            if ((this.lookupTerminalEligibilityCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.lookupTerminalEligibilityCompleted(this, new lookupTerminalEligibilityCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.dentalxchange.com/webservice", ResponseNamespace="http://www.dentalxchange.com/webservice")]
        [return: System.Xml.Serialization.SoapElementAttribute("lookupTerminalClaimStatusReturn")]
        public Response lookupTerminalClaimStatus(Credentials in0, Request in1) {
            object[] results = this.Invoke("lookupTerminalClaimStatus", new object[] {
                        in0,
                        in1});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void lookupTerminalClaimStatusAsync(Credentials in0, Request in1) {
            this.lookupTerminalClaimStatusAsync(in0, in1, null);
        }
        
        /// <remarks/>
        public void lookupTerminalClaimStatusAsync(Credentials in0, Request in1, object userState) {
            if ((this.lookupTerminalClaimStatusOperationCompleted == null)) {
                this.lookupTerminalClaimStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnlookupTerminalClaimStatusOperationCompleted);
            }
            this.InvokeAsync("lookupTerminalClaimStatus", new object[] {
                        in0,
                        in1}, this.lookupTerminalClaimStatusOperationCompleted, userState);
        }
        
        private void OnlookupTerminalClaimStatusOperationCompleted(object arg) {
            if ((this.lookupTerminalClaimStatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.lookupTerminalClaimStatusCompleted(this, new lookupTerminalClaimStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.dentalxchange.com/webservice", ResponseNamespace="http://www.dentalxchange.com/webservice")]
        [return: System.Xml.Serialization.SoapElementAttribute("updateTerminalReturn")]
        public Response updateTerminal(Credentials in0, Request in1) {
            object[] results = this.Invoke("updateTerminal", new object[] {
                        in0,
                        in1});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void updateTerminalAsync(Credentials in0, Request in1) {
            this.updateTerminalAsync(in0, in1, null);
        }
        
        /// <remarks/>
        public void updateTerminalAsync(Credentials in0, Request in1, object userState) {
            if ((this.updateTerminalOperationCompleted == null)) {
                this.updateTerminalOperationCompleted = new System.Threading.SendOrPostCallback(this.OnupdateTerminalOperationCompleted);
            }
            this.InvokeAsync("updateTerminal", new object[] {
                        in0,
                        in1}, this.updateTerminalOperationCompleted, userState);
        }
        
        private void OnupdateTerminalOperationCompleted(object arg) {
            if ((this.updateTerminalCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.updateTerminalCompleted(this, new updateTerminalCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://www.dentalxchange.com/webservice", ResponseNamespace="http://www.dentalxchange.com/webservice")]
        [return: System.Xml.Serialization.SoapElementAttribute("lookupClaimReturn")]
        public Response lookupClaim(Credentials in0, Request in1) {
            object[] results = this.Invoke("lookupClaim", new object[] {
                        in0,
                        in1});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void lookupClaimAsync(Credentials in0, Request in1) {
            this.lookupClaimAsync(in0, in1, null);
        }
        
        /// <remarks/>
        public void lookupClaimAsync(Credentials in0, Request in1, object userState) {
            if ((this.lookupClaimOperationCompleted == null)) {
                this.lookupClaimOperationCompleted = new System.Threading.SendOrPostCallback(this.OnlookupClaimOperationCompleted);
            }
            this.InvokeAsync("lookupClaim", new object[] {
                        in0,
                        in1}, this.lookupClaimOperationCompleted, userState);
        }
        
        private void OnlookupClaimOperationCompleted(object arg) {
            if ((this.lookupClaimCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.lookupClaimCompleted(this, new lookupClaimCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="http://www.dentalxchange.com/webservice")]
    public partial class Credentials {
        
        private string clientField;
        
        private string passwordField;
        
        private string serviceIDField;
        
        private string usernameField;
        
        private string versionField;
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable=true)]
        public string client {
            get {
                return this.clientField;
            }
            set {
                this.clientField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable=true)]
        public string password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable=true)]
        public string serviceID {
            get {
                return this.serviceIDField;
            }
            set {
                this.serviceIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable=true)]
        public string username {
            get {
                return this.usernameField;
            }
            set {
                this.usernameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable=true)]
        public string version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="http://www.dentalxchange.com/webservice")]
    public partial class Response {
        
        private string contentField;
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable=true)]
        public string content {
            get {
                return this.contentField;
            }
            set {
                this.contentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="http://www.dentalxchange.com/webservice")]
    public partial class Request {
        
        private string contentField;
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable=true)]
        public string content {
            get {
                return this.contentField;
            }
            set {
                this.contentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void lookupEligibilityCompletedEventHandler(object sender, lookupEligibilityCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class lookupEligibilityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal lookupEligibilityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void lookupClaimStatusCompletedEventHandler(object sender, lookupClaimStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class lookupClaimStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal lookupClaimStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void lookupFamilyEligibilityCompletedEventHandler(object sender, lookupFamilyEligibilityCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class lookupFamilyEligibilityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal lookupFamilyEligibilityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void lookupTerminalEligibilityCompletedEventHandler(object sender, lookupTerminalEligibilityCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class lookupTerminalEligibilityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal lookupTerminalEligibilityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void lookupTerminalClaimStatusCompletedEventHandler(object sender, lookupTerminalClaimStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class lookupTerminalClaimStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal lookupTerminalClaimStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void updateTerminalCompletedEventHandler(object sender, updateTerminalCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class updateTerminalCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal updateTerminalCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void lookupClaimCompletedEventHandler(object sender, lookupClaimCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class lookupClaimCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal lookupClaimCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591