# GHPT
This project sets out to find a way to leverage the power of ChatGPT to create Grasshopper definitions.
It was developed at the AEC Tech Seattle Hackathon 2023 hosted by Thornton Tomasetti CORE studio and LMN

![0 1 4_README](https://github.com/enmerk4r/GHPT/assets/9583495/91b22bdd-ac9f-4f0e-8a1a-727ddb44ff53)

## Team
- [Callum Sykes](https://github.com/clicketyclackety) - StructureCraft
- [Jo Kamm](https://github.com/jkamm) - Dimensional Innovations
- [Sergey Pigach](https://github.com/enmerk4r) - Thornton Tomasetti
- [Ryan Erbert](https://github.com/RyanErbert)
- [Quoc Dang](https://github.com/jackDang2803)

## Installation
Your options for installing GHPT are as follows:
- Install the plug-in from [Food4Rhino](https://www.food4rhino.com/en/app/ghpt).
- Install the GHPT Yak package from Rhino's Package Manager.
- Build from source.

## Token Configuration
Once GhPT is downloaded and installed, the OpenAI token key needs to be set up through the pop-up Token Configuration Window. To use the OpenAI API, you need to provide an API key and specify a GPT model.
![0 1 4_README_API](https://github.com/enmerk4r/GHPT/assets/9583495/117d2ee2-82f8-44a8-bc57-26baa23fc1c3)


You can sign up for OpenAI API on [this page](https://openai.com/product). Once you signed up and logged in, open [this page](https://platform.openai.com/account/api-keys) and select Create new secret key. You can then copy the key by clicking on the green text Copy, make sure to save this key somewhere else as you will not be able to access it again.
![image](https://github.com/enmerk4r/GHPT/assets/114206649/66441be3-3c87-4de1-81ca-71a1565347ce)


Paste the key in the GPT Token box.

![image](https://github.com/enmerk4r/GHPT/assets/9583495/96648b0a-3cdb-4662-8b8f-d92c97a8b417)


Go to [this page](https://platform.openai.com/account/rate-limits) to check for your access to different GPT models (currently we are using the more advanced model GPT-4 that has limited access) but model GPT-3.5 should also work well.
![gpt model](https://github.com/enmerk4r/GHPT/assets/114206649/fd61e092-9a65-484b-b394-93e22a1263cf)


Select your model from the dropdown menu

![image](https://github.com/enmerk4r/GHPT/assets/9583495/6c01db32-d336-4848-aa67-3fa7a2129b2c)



## How to use

Creating a component and initiating a request to the ChatGPT API can be accomplished via shortcut; Prompts can be written directly into the grasshopper component search function.

![image](https://github.com/enmerk4r/GHPT/assets/9583495/4cb263b5-6a15-4f1d-98cf-4d6e65c93977)

To take advantage of this functionality, double click an empty space on the canvas and type a prompt in the following format:

`GHPT = <your prompt goes here>`

After allowing the module some time to think, an organized node graph will appear. Additionally, an "Advice" text panel will be create to display advice/feedback from GPT.

In the event that a prompt is too complex, the module will display an error message.


Prompt guide - add these text snippet at the end of your request for more specific instructions to GPT

* "if there are questions, put them in the Advice section" -> if your prompt is too complex for GPT, this allow GPT to ask for clarification
* "prefer Circle over Circle CNR" -> instruct GPT to prefer a component over another one with similar name/function
* "be specific" -> more clarified
