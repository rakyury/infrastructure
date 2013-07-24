﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Spark.Infrastructure.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Exceptions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Exceptions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Spark.Infrastructure.Resources.Exceptions", typeof(Exceptions).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of Trace.CorrelationManager.ActivityId is not the ActivityId value set by this NestedDiagnosticContext..
        /// </summary>
        internal static string ActivityIdModifiedInsideScope {
            get {
                return ResourceManager.GetString("ActivityIdModifiedInsideScope", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aggregate must have a single explicit ApplyByStrategyAttribute defined.
        ///Aggregate Type: {0}.
        /// </summary>
        internal static string AggregateAmbiguousApplyMethodStrategy {
            get {
                return ResourceManager.GetString("AggregateAmbiguousApplyMethodStrategy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A method marked with ApplyMethodAttribute must have a single input parameter that derives from {0}.
        ///Reflected Type: {1}
        ///Method Name: {2}.
        /// </summary>
        internal static string AggregateApplyMethodInvalidParameters {
            get {
                return ResourceManager.GetString("AggregateApplyMethodInvalidParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A method marked with ApplyMethodAttribute must return void.
        ///Reflected Type: {0}
        ///Method Name: {1}.
        /// </summary>
        internal static string AggregateApplyMethodMustHaveVoidReturn {
            get {
                return ResourceManager.GetString("AggregateApplyMethodMustHaveVoidReturn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aggregate apply method not found for event type.
        ///Aggregate Type: {0}
        ///Event Type: {1}.
        /// </summary>
        internal static string AggregateApplyMethodNotFound {
            get {
                return ResourceManager.GetString("AggregateApplyMethodNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aggregate must have a single public default constructor.
        ///Aggregate Type: {0}.
        /// </summary>
        internal static string AggregateDefaultConstructorRequired {
            get {
                return ResourceManager.GetString("AggregateDefaultConstructorRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ambiguous command handler mapping strategy,
        ///Aggregate Type: {0}.
        /// </summary>
        internal static string AggregateHandleByStrategyAmbiguous {
            get {
                return ResourceManager.GetString("AggregateHandleByStrategyAmbiguous", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aggregate type was not discovered by type locator.
        ///Aggregate Type: {0}.
        /// </summary>
        internal static string AggregateTypeUndiscovered {
            get {
                return ResourceManager.GetString("AggregateTypeUndiscovered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must not equal {0}..
        /// </summary>
        internal static string ArgumentEqualToValue {
            get {
                return ResourceManager.GetString("ArgumentEqualToValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must equal {0}..
        /// </summary>
        internal static string ArgumentNotEqualToValue {
            get {
                return ResourceManager.GetString("ArgumentNotEqualToValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must be greater than or equal to {0}..
        /// </summary>
        internal static string ArgumentNotGreaterThanOrEqualToValue {
            get {
                return ResourceManager.GetString("ArgumentNotGreaterThanOrEqualToValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must be greater than {0}..
        /// </summary>
        internal static string ArgumentNotGreaterThanValue {
            get {
                return ResourceManager.GetString("ArgumentNotGreaterThanValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must be less than or equal to {0}..
        /// </summary>
        internal static string ArgumentNotLessThanOrEqualToValue {
            get {
                return ResourceManager.GetString("ArgumentNotLessThanOrEqualToValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must be less than {0}..
        /// </summary>
        internal static string ArgumentNotLessThanValue {
            get {
                return ResourceManager.GetString("ArgumentNotLessThanValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This CommandContext is being disposed on a different thread than it was created..
        /// </summary>
        internal static string CommandContextInterleaved {
            get {
                return ResourceManager.GetString("CommandContextInterleaved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This CommandContext is being disposed out of order..
        /// </summary>
        internal static string CommandContextInvalidThread {
            get {
                return ResourceManager.GetString("CommandContextInvalidThread", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command not handled by any known aggregate type.
        ///Command Type: {0}.
        /// </summary>
        internal static string CommandHandlerNotFound {
            get {
                return ResourceManager.GetString("CommandHandlerNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The commit operation has timed out.
        ///Commit Id: {0}
        ///Stream Id: {1}.
        /// </summary>
        internal static string CommitTimeout {
            get {
                return ResourceManager.GetString("CommitTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Concurrency Exception.
        ///Stream Id: {0}
        ///Version: {1}.
        /// </summary>
        internal static string ConcurrencyException {
            get {
                return ResourceManager.GetString("ConcurrencyException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection string not found.
        ///Name: {0}.
        /// </summary>
        internal static string ConnectionNotFound {
            get {
                return ResourceManager.GetString("ConnectionNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection string provider not specified.
        ///Name: {0}.
        /// </summary>
        internal static string ConnectionProviderNotSpecified {
            get {
                return ResourceManager.GetString("ConnectionProviderNotSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicate commit.
        ///Commit Id: {0}.
        /// </summary>
        internal static string DuplicateCommitException {
            get {
                return ResourceManager.GetString("DuplicateCommitException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This EventContext is being disposed on a different thread than it was created..
        /// </summary>
        internal static string EventContextInterleaved {
            get {
                return ResourceManager.GetString("EventContextInterleaved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This EventContext is being disposed out of order..
        /// </summary>
        internal static string EventContextInvalidThread {
            get {
                return ResourceManager.GetString("EventContextInvalidThread", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ambiguous event handler mapping strategy,
        ///Handler Type: {0}.
        /// </summary>
        internal static string EventHandlerHandleByStrategyAmbiguous {
            get {
                return ResourceManager.GetString("EventHandlerHandleByStrategyAmbiguous", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A method marked with HandleMethodAttribute must have the first input parameter derive from {0}.
        ///Reflected Type: {1}
        ///Method Name: {2}.
        /// </summary>
        internal static string HandleMethodInvalidParameters {
            get {
                return ResourceManager.GetString("HandleMethodInvalidParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A command may only be handled by a single aggregate type.
        ///Aggregate Type: {0}
        ///Command Type: {1}.
        /// </summary>
        internal static string HandleMethodMustBeAssociatedWithSingleAggregate {
            get {
                return ResourceManager.GetString("HandleMethodMustBeAssociatedWithSingleAggregate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A method marked with HandleMethodAttribute must return void.
        ///Reflected Type: {0}
        ///Method Name: {1}.
        /// </summary>
        internal static string HandleMethodMustHaveVoidReturn {
            get {
                return ResourceManager.GetString("HandleMethodMustHaveVoidReturn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Overloaded method not allowed.
        ///Reflected Type: {0}
        ///Method: {1}.
        /// </summary>
        internal static string HandleMethodOverloaded {
            get {
                return ResourceManager.GetString("HandleMethodOverloaded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The current HTTP context is not available on this thread..
        /// </summary>
        internal static string HttpContextNotAvailable {
            get {
                return ResourceManager.GetString("HttpContextNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing one or more aggregate commits; unable to apply commit to aggregate.
        ///Expected Version: {0}
        ///Actual Version: {1}.
        /// </summary>
        internal static string MissingAggregateCommits {
            get {
                return ResourceManager.GetString("MissingAggregateCommits", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must contain at least one non-whitespace character..
        /// </summary>
        internal static string MustContainOneNonWhitespaceCharacter {
            get {
                return ResourceManager.GetString("MustContainOneNonWhitespaceCharacter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The current CommandContext is null..
        /// </summary>
        internal static string NoCommandContext {
            get {
                return ResourceManager.GetString("NoCommandContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The current EventContext is null..
        /// </summary>
        internal static string NoEventContext {
            get {
                return ResourceManager.GetString("NoEventContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of Trace.CorrelationManager.LogicalOperationStack is not the OperationId value set by this NestedDiagnosticContext..
        /// </summary>
        internal static string OperationIdModifiedInsideScope {
            get {
                return ResourceManager.GetString("OperationIdModifiedInsideScope", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Maximum page size of {0} has been exceeded..
        /// </summary>
        internal static string PageSizeExceeded {
            get {
                return ResourceManager.GetString("PageSizeExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter source column not specified.
        ///Parameter Name: {0}.
        /// </summary>
        internal static string ParameterSourceColumnNotSet {
            get {
                return ResourceManager.GetString("ParameterSourceColumnNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reserved system header.
        ///Name: {0}.
        /// </summary>
        internal static string ReservedHeaderName {
            get {
                return ResourceManager.GetString("ReservedHeaderName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Event handler must have a single public default constructor.
        ///Handler Type: {0}.
        /// </summary>
        internal static string SagaDefaultConstructorRequired {
            get {
                return ResourceManager.GetString("SagaDefaultConstructorRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to State may only be changed via an event apply method when saving an aggregate.
        ///Aggregate Id: {0}.
        /// </summary>
        internal static string StateAccessException {
            get {
                return ResourceManager.GetString("StateAccessException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type cannot be an interface..
        /// </summary>
        internal static string TypeArgumentMustNotBeAnInterface {
            get {
                return ResourceManager.GetString("TypeArgumentMustNotBeAnInterface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type does not derive from {0}.
        ///Type: {1}.
        /// </summary>
        internal static string TypeDoesNotDeriveFromBase {
            get {
                return ResourceManager.GetString("TypeDoesNotDeriveFromBase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown database dialect.
        ///Provider Name: {0}.
        /// </summary>
        internal static string UnknownDialect {
            get {
                return ResourceManager.GetString("UnknownDialect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unresolved Concurrency conflict: {0}.
        /// </summary>
        internal static string UnresolvedConcurrencyConflict {
            get {
                return ResourceManager.GetString("UnresolvedConcurrencyConflict", resourceCulture);
            }
        }
    }
}
