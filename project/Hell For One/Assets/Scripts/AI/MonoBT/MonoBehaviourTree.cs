using System;
using System.Linq;

namespace Ai.MonoBT
{
    public class BehaviourTree : IBtTask
    {
        protected IBtTask root;

        public BehaviourTree(IBtTask task)
        {
            root = task;
        }

        public bool Run()
        {
            return root.Run();
        }
    }
    
    public delegate bool BtCAll();

    public interface IBtTask
    {
        bool Run();
    }

    public abstract class BtDecorator : IBtTask
    {
        protected IBtTask child;

        public BtDecorator(IBtTask task)
        {
            child = task;
        }

        public abstract bool Run();
    }

    public class BtCondition : IBtTask
    {
        protected BtCAll Condition;

        public BtCondition(BtCAll call)
        {
            Condition = call;
        }

        public bool Run()
        {
            return Condition();
        }
    }

    public class BtAction : IBtTask
    {
        protected BtCAll Action;

        public BtAction(BtCAll call)
        {
            Action = call;
        }

        public bool Run()
        {
            return Action();
        }
    }

    public abstract class BtComposite : IBtTask
    {
        protected IBtTask[] Children;

        public BtComposite(IBtTask[] tasks)
        {
            Children = tasks;
        }

        public abstract bool Run();
    }

    public abstract class BtRandomComposite : BtComposite
    {
        protected BtRandomComposite(IBtTask[] tasks) : base(tasks)
        {
        }

        public void Shuffle()
        {
            Random rnd = new Random();
            Children = Children.OrderBy(x => rnd.Next()).ToArray();
        }
    }

    public class BtSelector : BtComposite
    {
        public BtSelector(IBtTask[] tasks) : base(tasks)
        {
        }

        public override bool Run()
        {
            foreach (IBtTask task in Children)
            {
                if (task.Run()) 
                    return true;
            }

            return false;
        }
    }

    public class BtSequence : BtComposite
    {
        public BtSequence(IBtTask[] tasks) : base(tasks)
        {
        }

        public override bool Run()
        {
            foreach (IBtTask task in Children)
            {
                if (!task.Run())
                    return false;
            }

            return true;
        }
    }
    
    public class BtRandomSelector : BtRandomComposite
    {
        public BtRandomSelector(IBtTask[] tasks) : base(tasks)
        {
        }

        public override bool Run()
        {
            Shuffle();
            foreach (IBtTask task in  Children)
            {
                if (task.Run()) return true;
            }

            return false;
        }
    }

    public class BtRandomSequence : BtRandomComposite
    {
        public BtRandomSequence(IBtTask[] tasks) : base(tasks)
        {
        }

        public override bool Run()
        {
            Shuffle();
            foreach (IBtTask task in Children)
            {
                if (!task.Run())
                    return false;
            }

            return true;
        }
    }

    public class BtDecoratorFilter : BtDecorator
    {
        private BtCAll Condition;
        
        public BtDecoratorFilter(BtCAll condition, IBtTask task) : base(task)
        {
            Condition = condition;
        }

        public override bool Run()
        {
            return Condition() && child.Run();
        }
    }

    public class BtDecoratorLimit : BtDecorator
    {
        private readonly int _maxRepetitions;
        private int _count;
        
        public BtDecoratorLimit(int max, IBtTask task) : base(task)
        {
            _maxRepetitions = max;
            _count = 0;
        }

        public override bool Run()
        {
            if (_count < _maxRepetitions)
            {
                _count += 1;
                return child.Run();
            }

            return true;
        }
    }
    
    public class BtDecoratorUntilFail : BtDecorator {
		
        public BtDecoratorUntilFail(IBtTask task) : base(task) { ; }

        public override bool Run()
        {
            while (child.Run())
            {
                
            } 
            return true;
        }
    }

    // A task implementing an inverter decorator
    public class BtDecoratorInverter : BtDecorator {
		
        public BtDecoratorInverter(IBtTask task) : base(task) { ; }

        public override bool Run()
        {
            return !child.Run();
        }
    }
}