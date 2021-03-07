﻿module Elmish.Uno.Samples.SubModel.Program

open System
open Serilog
open Serilog.Extensions.Logging
open Elmish
open Elmish.Uno

module Counter =

  type Model =
    { Count: int
      StepSize: int }

  type Msg =
    | Increment
    | Decrement
    | SetStepSize of int
    | Reset

  let init =
    { Count = 0
      StepSize = 1 }

  let canReset = (<>) init

  let update msg m =
    match msg with
    | Increment -> { m with Count = m.Count + m.StepSize }
    | Decrement -> { m with Count = m.Count - m.StepSize }
    | SetStepSize x -> { m with StepSize = x }
    | Reset -> init

  let bindings () : Binding<Model, Msg> list = [
    "CounterValue" |> Binding.oneWay (fun m -> m.Count)
    "Increment" |> Binding.cmd Increment
    "Decrement" |> Binding.cmd Decrement
    "StepSize" |> Binding.twoWay(
      (fun m -> float m.StepSize),
      int >> SetStepSize)
    "Reset" |> Binding.cmdIf(Reset, canReset)
  ]


module Clock =

  type TimeType =
    | Utc
    | Local

  type Model =
    { Time: DateTimeOffset
      TimeType: TimeType }

  let init () =
    { Time = DateTimeOffset.Now
      TimeType = Local }

  let getTime m =
    match m.TimeType with
    | Utc -> m.Time.UtcDateTime
    | Local -> m.Time.LocalDateTime

  type Msg =
    | Tick of DateTimeOffset
    | SetTimeType of TimeType

  let update msg m =
    match msg with
    | Tick t -> { m with Time = t }
    | SetTimeType t -> { m with TimeType = t }

  let bindings () : Binding<Model, Msg> list = [
    "Time" |> Binding.oneWay getTime
    "IsLocal" |> Binding.oneWay (fun m -> m.TimeType = Local)
    "SetLocal" |> Binding.cmd (SetTimeType Local)
    "IsUtc" |> Binding.oneWay (fun m -> m.TimeType = Utc)
    "SetUtc" |> Binding.cmd (SetTimeType Utc)
  ]


module CounterWithClock =

  type Model =
    { Counter: Counter.Model
      Clock: Clock.Model }

  let init () =
    { Counter = Counter.init
      Clock = Clock.init () }

  type Msg =
    | CounterMsg of Counter.Msg
    | ClockMsg of Clock.Msg

  let update msg m =
    match msg with
    | CounterMsg msg -> { m with Counter = Counter.update msg m.Counter }
    | ClockMsg msg -> { m with Clock = Clock.update msg m.Clock }

  let bindings () : Binding<Model, Msg> list = [
    "Counter" |> Binding.subModel((fun m -> m.Counter), snd, CounterMsg, Counter.bindings)
    "Clock" |> Binding.subModel((fun m -> m.Clock), snd, ClockMsg, Clock.bindings)
  ]


module App =

  type Model =
    { ClockCounter1: CounterWithClock.Model
      ClockCounter2: CounterWithClock.Model }

  let init () =
    { ClockCounter1 = CounterWithClock.init ()
      ClockCounter2 = CounterWithClock.init () }

  type Msg =
    | ClockCounter1Msg of CounterWithClock.Msg
    | ClockCounter2Msg of CounterWithClock.Msg

  let update msg m =
    match msg with
    | ClockCounter1Msg msg ->
        { m with ClockCounter1 = CounterWithClock.update msg m.ClockCounter1 }
    | ClockCounter2Msg msg ->
        { m with ClockCounter2 = CounterWithClock.update msg m.ClockCounter2 }

  let bindings () : Binding<Model, Msg> list = [
    "ClockCounter1" |> Binding.subModel(
      (fun m -> m.ClockCounter1),
      snd,
      ClockCounter1Msg,
      CounterWithClock.bindings)

    "ClockCounter2" |> Binding.subModel(
      (fun m -> m.ClockCounter2),
      snd,
      ClockCounter2Msg,
      CounterWithClock.bindings)
  ]


let counterDesignVm = ViewModel.designInstance Counter.init (Counter.bindings ())
let clockDesignVm = ViewModel.designInstance (Clock.init ()) (Clock.bindings ())
let counterWithClockDesignVm = ViewModel.designInstance (CounterWithClock.init ()) (CounterWithClock.bindings ())
let mainDesignVm = ViewModel.designInstance (App.init ()) (App.bindings ())


let timerTick dispatch =
  let timer = new System.Timers.Timer(1000.)
  timer.Elapsed.Add (fun _ ->
    let clockMsg =
      DateTimeOffset.Now
      |> Clock.Tick
      |> CounterWithClock.ClockMsg
    dispatch <| App.ClockCounter1Msg clockMsg
    dispatch <| App.ClockCounter2Msg clockMsg
  )
  timer.Start()


let main window =

  let logger =
    LoggerConfiguration()
      .MinimumLevel.Override("Elmish.Uno.Update", Events.LogEventLevel.Verbose)
      .MinimumLevel.Override("Elmish.Uno.Bindings", Events.LogEventLevel.Verbose)
      .MinimumLevel.Override("Elmish.Uno.Performance", Events.LogEventLevel.Verbose)
      .WriteTo.Console()
      .CreateLogger()

  Program.mkSimple App.init App.update App.bindings
  |> Program.withSubscription (fun _ -> Cmd.ofSub timerTick)
  |> Program.withLogger (new SerilogLoggerFactory(logger))
  |> Program.startElmishLoop window
