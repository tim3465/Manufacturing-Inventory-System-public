Machines Tests Only

Scope:
This task applies only to CncApp.Application.Tests/Services/Machines.

Goal:
Align the file and folder structure only of Machines tests to mirror CncApp.Application.Services.Machines.

Target Structure:

CncApp.Application.Tests
└─ Services
   └─ Machines
      ├─ Commands
      │  ├─ MachineTests.Create.cs
      │  └─ MachineTests.Inactivate.cs
      ├─ Queries
      │  ├─ MachineTests.Get.cs
      │  ├─ MachineTests.ListActive.cs
      │  └─ MachineTests.ListAll.cs
      └─ MachineTests.cs

Hard Rules (Do Not Violate)

Do NOT move, copy, merge, or redistribute existing test methods.

Do NOT infer or regroup tests by behavior.

Each file must contain tests for exactly one operation, based on the filename:

Create → Create tests only

Inactivate → Inactivate tests only

Get → Get tests only

ListActive → ListActive tests only

ListAll → ListAll tests only

If a file does not yet have tests for its operation, create it as an empty placeholder only.

All files must be partial classes derived from MachineTests.

MachineTests.cs may contain shared setup only (mocks, service initialization).

No test methods belong in MachineTests.cs.

Implementation Instructions

Create missing files as empty placeholders.

Convert existing Machines test files to partial class MachineTests without changing method bodies.

If an existing test does not clearly belong to a file by name, leave it where it is and do not move it.

Ensure all files follow the Global C# File Header Rules (using lines at top, one namespace, no compression).

Restriction

Do not change any other files or entities.

Do not refactor logic.

Do not “improve” organization beyond what is explicitly stated.