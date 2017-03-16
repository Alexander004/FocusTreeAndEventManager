﻿using FocusTreeManager.CodeStructures.CodeExceptions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace FocusTreeManager.CodeStructures
{
    [KnownType(typeof(Assignation))]
    [KnownType(typeof(CodeBlock))]
    [KnownType(typeof(CodeValue))]
    [DataContract(Name = "code_value")]
    public class CodeValue : ICodeStruct
    {
        [DataMember(Name = "value", Order = 0)]
        public string Value { get; set; }

        public CodeValue()
        { }

        public CodeValue(string value)
        {
            Value = value;
        }

        internal void Analyse(SyntaxGroup code, int line = -1)
        {
            //Can't analyse
        }

        public CodeValue FindValue(string TagToFind)
        {
            //End of structure
            return null;
        }

        public ICodeStruct Extract(string TagToFind)
        {
            //End of structure
            return null;
        }

        public Assignation FindAssignation(string TagToFind)
        {
            //End of structure
            return null;
        }

        public List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind)
        {
            //End of structure
            return new List<ICodeStruct>();
        }

        public string Parse(int StartLevel = -1)
        {
            return Value;
        }

        public Script GetContentAsScript(string[] except)
        {
            //TODO: Add language support
            RecursiveCodeException e = new RecursiveCodeException();
            throw e.AddToRecursiveChain("Impossible to obtain content, value has not code",
                                            Value, "-1");
        }
    }
}
