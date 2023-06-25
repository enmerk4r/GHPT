# Developer Guide

GHPT utilises the power of ChatGPT, you can use whichever version you like, although gpt-4 yields considerably better results.



## Prompt Engineering

If you're newer to prompt engineering, you should read [this guide](https://www.promptingguide.ai/).

GHPT uses [`few-shot`](https://www.promptingguide.ai/techniques/fewshot) and [`chain-of-thought`](https://www.promptingguide.ai/techniques/cot) prompting to ensure correct, well thought out and consistent responses.



### Few-Shot

This is where we give ChatGPT examples of what we want, and then let it fill in the blanks. ChatGPT is very good at filling in the blanks, and given proper context, will return information in whatever format you require. The central part of how GHPT works is to get ChatGPT to return its ideas in a Json Schema that matches the serialization and deserialization of our `PromptData` struct.



### Chain-of-thought

You may have used ChatGPT and it gave you a strange response, used mathematics incorrectly, or seemingly used some obvious logical fallacies. Chain-of-thought assists here, and works hand in hand with few-shot prompting. In the case of GHPT we give ChatGPT a few examples of how we'd like it to respond, and the logic we used in solving our few-shot examples.



## Unit Testing

Often unit testing can be the best way to explore a new repo. It helps you work with the more testable readable code, everyone appreciates someone else writing unit tests and it lets you quickly contribute to the repo safe from creating potentially breaking changes.



GHPT has a couple of unit tests to begin, testing the serialization schema and that ChatGPT can be reached correctly. To run unit tests you'll need to copy the ghpt.json from the GHPT/bin directory to the unit test directory otherwise it will not contact the server.



## Tokens & OpenAI Authentication

Make sure to place the GHPT Component on the canvas once to set up the token before testing.