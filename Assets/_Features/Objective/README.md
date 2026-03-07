Refer to Figma Design for more details.


To add load a new objective into system, the main entrypoint is through a LoadObjectiveRequest (IGlobalEvent) to event bus.

Note, the system can only handle one active tutorial at a time. If a LoadObjectiveRequest is sent when there is already a current
active objective, that request will be ignored.


Ways to LoadObjectiveRequests --> attach an EventInvoker component to a monobehaviour object, and then send LoadObjectiveRequest OnEnable of that gameobject.