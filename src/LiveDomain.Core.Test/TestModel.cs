﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveDomain.Core.Proxy;

namespace LiveDomain.Core.Test
{
    [Serializable]
    public class TestModel : Model
    {
        public int CommandsExecuted { get; set; }

        public bool OnLoadExecuted { get; private set; }
        
        protected internal override void JournalRestored()
        {
            OnLoadExecuted = true;
        }

		/// <summary>
		/// This will be a Command if called via Proxy
		/// </summary>
    	public void IncreaseNumber()
    	{
    		CommandsExecuted++;
    	}

		/// <summary>
		/// This will be a CommandWithResult if called via Proxy
		/// </summary>
		/// <param name="livedb"></param>
		/// <returns></returns>
		[ProxyMethod(ProxyMethodType.Command)]
    	public string Uppercase(string livedb)
		{
			CommandsExecuted++;
    		return livedb.ToUpper();
    	}

		/// <summary>
		/// This is only for test and should return SerializationException since we can't use IEnumerable with yield.
		/// </summary>
		/// <returns></returns>
    	public IEnumerable<string> GetNames()
    	{
    		for (int i = 0; i < 10; i++)
    		{
    			yield return i.ToString();
    		}
    	}

		/// <summary>
		/// This will be a Query if called via Proxy.
		/// </summary>
		/// <returns></returns>
    	public int GetNumber()
    	{
    		return CommandsExecuted;
    	}
    }


    public class GetNumberOfCommandsExecutedQuery : Query<TestModel, int>
    {
        protected override int Execute(TestModel model)
        {
            return model.CommandsExecuted;
        }
    }

    [Serializable]
    public class TestCommandWithoutResult : Command<TestModel>
    {
        protected internal override void Execute(TestModel model)
        {
            model.CommandsExecuted++;
        }
    }

    [Serializable]
    public class TestCommandWithResult : CommandWithResult<TestModel,int>
    {
        public byte[] Payload { get; set; }
        public bool ThrowInPrepare { get; set; }
        public bool ThrowExceptionInExecute { get; set; }
        public bool ThrowCommandFailedExceptionFromExecute { get; set; }

        protected internal override void Prepare(TestModel model)
        {
            if (ThrowInPrepare)
            {
                throw new Exception();
            }
        }
        protected internal override int Execute(TestModel model)
        {
            if (ThrowCommandFailedExceptionFromExecute)
            {
                throw new CommandAbortedException();
            }
            if (ThrowExceptionInExecute)
            {
                throw new Exception();
            }
            return ++model.CommandsExecuted;
        }

    }
}
