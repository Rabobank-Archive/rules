using SecurePipelineScan.VstsService.Response;
using System;
using SecurePipelineScan.VstsService;
using System.Collections.Generic;

namespace SecurePipelineScan.Rules.Security
{

    public class PipelineEvaluatorFactory
    {

        private readonly Dictionary<int, IPipelineEvaluator> _evaluators;

        public PipelineEvaluatorFactory(IVstsRestClient client)
        {
            _evaluators = new Dictionary<int, IPipelineEvaluator>
            {
                [GuiPipelineProcessType] = new GuiPipelineEvaluator(client),
                [YamlPipelineProcessType] = new YamlPipelineEvaluator(client)
            };
        }

        private const int GuiPipelineProcessType = 1;
        private const int YamlPipelineProcessType = 2;

        public IPipelineEvaluator Create(BuildDefinition buildPipeline)
        {
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));
            if (!_evaluators.ContainsKey(buildPipeline.Process.Type))
                throw new ArgumentOutOfRangeException(nameof(buildPipeline));

            return _evaluators[buildPipeline.Process.Type];
        }
    }
}