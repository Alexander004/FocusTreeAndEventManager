﻿using FocusTreeManager.CodeStructures.CodeExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FocusTreeManager.CodeStructures
{
    [KnownType(typeof(Assignation))]
    [KnownType(typeof(CodeBlock))]
    [KnownType(typeof(CodeValue))]
    [DataContract(Name = "assignation")]
    public class Assignation : ICodeStruct
    {
        [DataMember(Name = "assignee", Order = 0)]
        public string Assignee { get; set; }

        [DataMember(Name = "value", Order = 1)]
        public ICodeStruct Value { get; set; }

        [DataMember(Name = "level", Order = 2)]
        private int Level;

        [DataMember(Name = "operator", Order = 3)]
        public string Operator { get; set; }

        [DataMember(Name = "line", Order = 4)]
        public int Line { get; set; }

        public Assignation()
        {
            Level = 0;
        }

        public Assignation(int level)
        {
            Level = level;
        }

        internal void Analyse(SyntaxGroup code)
        {
            Assignee = code.Component.text;
            Operator = code.Operator.text;
            Line = code.Component.line;
            //If we can detect at least one code block
            List<SyntaxGroup> list = code.Operand as List<SyntaxGroup>;
            if (list != null)
            {
                try
                {
                    CodeBlock block = new CodeBlock(Level + 1);
                    block.Analyse(list);
                    Value = block;
                }
                catch (RecursiveCodeException e)
                {
                    //TODO: Add language support
                    throw e.AddToRecursiveChain("Error during analysis chain", 
                        Assignee, Line.ToString());
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    RecursiveCodeException e = new RecursiveCodeException();
                    throw e.AddToRecursiveChain("Impossible to analyze associated code", 
                        Assignee, Line.ToString());
                }
            }
            //If we get pure text
            else if (code.Operand is Token)
            {
                Value = new CodeValue(((Token)code.Operand).text);
            }
            //If we got a list of tokens, a chain of pure text
            else if (code.Operand is List<Token>)
            {
                Value = new CodeBlock();
                ((CodeBlock)Value).Analyse((List<Token>)code.Operand);
            }
            else
            {
                //Empty, kill
            }
        }

        public string Parse(int StartLevel = -1)
        {
            int BasicLevel = StartLevel == -1 ? Level : StartLevel + 1;
            string tabulations = "";
            for (int i = 1; i < BasicLevel; i++)
            {
                tabulations += "\t";
            }
            StringBuilder content = new StringBuilder();
            try
            {
                // If the value is nothing but it has an operator
                if ((Value == null || Value is CodeBlock
                    && !((CodeBlock)Value).Code.Any())
                    && Operator != null)
                {
                    //Empty block
                    content.Append(tabulations + Assignee + " " + Operator + " {\n\n}");
                }
                //Otherwise, print as usual
                else if (Value != null)
                {
                    content.Append(tabulations + Assignee + " " + Operator + " " + 
                        Value.Parse(BasicLevel));
                }
            }
            catch (RecursiveCodeException e)
            {
                //TODO: Add language support
                throw e.AddToRecursiveChain("Error during parsing chain", Assignee, Line.ToString());
            }
            catch (Exception)
            {
                //TODO: Add language support
                RecursiveCodeException e = new RecursiveCodeException();
                throw e.AddToRecursiveChain("Impossible to parse associated code", Assignee, Line.ToString());
            }
            return content.ToString();
        }

        public CodeValue FindValue(string TagToFind)
        {
            if (Assignee == TagToFind)
            {
                return Value as CodeValue;
            }
            //If we can,t run through the value, return unfound
            if (!(Value is CodeBlock)) return null;
            CodeValue found = Value.FindValue(TagToFind);
            return found;
        }

        public ICodeStruct Extract(string TagToFind)
        {
            if (Assignee == TagToFind)
            {
                return this;
            }
            //If value cannot be ran through, return unfound
            if (!(Value is CodeBlock)) return null;
            ICodeStruct found = Value.Extract(TagToFind);
            //Return what was found, cannot extract from an assignation, 
            //should extract in parent container
            return found;
        }

        public Assignation FindAssignation(string TagToFind)
        {
            if (Assignee == TagToFind)
            {
                return this;
            }
            //If value cannot be ran through, return unfound.
            if (!(Value is CodeBlock)) return null;
            Assignation found = Value.FindAssignation(TagToFind);
            return found;
        }

        public List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind)
        {
            List<ICodeStruct> founds = new List<ICodeStruct>();
            if (Value == null)
            {
                //Empty block (Paradox loves these....)
                return founds;
            }
            if (Assignee == TagToFind 
                && (Value.GetType() == typeof(T) ||
                Value is T))
            {
                founds.Add(Value);
                return founds;
            }
            //If we haven't found this element as our tag, search in childs
            if (Value is CodeBlock)
            {
                founds.AddRange(Value.FindAllValuesOfType<T>(TagToFind));
            }
            return founds;
        }

        public Script GetContentAsScript(string[] except)
        {
            Script newScript = new Script();
            if (Value is CodeBlock)
            {
                foreach (ICodeStruct codeStruct in ((CodeBlock)Value).Code)
                {
                    Assignation item = (Assignation)codeStruct;
                    if (!except.Contains(item.Assignee))
                    {
                        newScript.Code.Add(item);
                    }
                }
            }
            else
            {
                //TODO: Add language support
                RecursiveCodeException e = new RecursiveCodeException();
                throw e.AddToRecursiveChain("Impossible to obtain content, assigned value is not code", 
                                             Assignee, Line.ToString());
            }
            return newScript;
        }
    }
}
